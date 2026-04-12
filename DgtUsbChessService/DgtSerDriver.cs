using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DgtUsbChessService;
using Timer = System.Timers.Timer;

namespace Test;

public enum Figuren
{
    Frei = 0,
    WBauer = 1,
    WTurm = 2,
    WSpringer = 3,
    WLaeufer = 4,
    WKoenig = 5,
    WDame = 6,
    SBauer = 7,
    STurm = 8,
    SSpringer = 9,
    SLaeufer = 10,
    SKoenig = 11,
    SDame = 12
}

public class DgtSerDriver
{
    private SerialPort? _serialPort;
    private Timer? _timer;

    private byte[]? _lastBytes;
    private readonly List<byte> _buffer = new();

    public delegate void UpdateBoardEventHandler(byte[] data);

    public event UpdateBoardEventHandler UpdateBoard;

    public bool ConState { get; set; } = false;
    public bool MoveHelp { get; set; } = false;
    public FigurColor FigurColor { get; set; } = FigurColor.White;
    public List<string> Moves { get; set; } = new List<string>();
    public List<string> PossibleMoves { get; set; } = new List<string>();

    public static bool TestConnection(string comport)
    {
        try
        {
            using var testPort = new SerialPort(comport, 9600, Parity.None, 8, StopBits.One);
            testPort.ReadTimeout = 1500;
            testPort.WriteTimeout = 1500;
            
            // Set DTR/RTS (essential for many DGT adapters)
            testPort.DtrEnable = true;
            testPort.RtsEnable = true;

            testPort.Open();

            // Longer stabilization time to allow hardware reset/boot
            System.Threading.Thread.Sleep(1500);

            testPort.DiscardInBuffer();
            testPort.DiscardOutBuffer();

            // Try Send Board command (0x42)
            testPort.Write(new byte[] { 0x42 }, 0, 1);
            
            var sw = Stopwatch.StartNew();
            List<byte> testBuffer = new();
            while (sw.ElapsedMilliseconds < 1500)
            {
                if (testPort.BytesToRead > 0)
                {
                    byte[] incoming = new byte[testPort.BytesToRead];
                    int read = testPort.Read(incoming, 0, incoming.Length);
                    for(int i=0; i<read; i++) testBuffer.Add(incoming[i]);
                    
                    // Look for ANY DGT Header (0x80 bit set)
                    if (testBuffer.Any(b => (b & 0x80) != 0)) 
                    {
                        // Found a header byte, likely a DGT board
                        return true; 
                    }
                }
                System.Threading.Thread.Sleep(50);
            }
            
            // If 0x42 didn't work, try Send Serial command (0x40)
            testPort.Write(new byte[] { 0x40 }, 0, 1);
            sw.Restart();
            while (sw.ElapsedMilliseconds < 1000)
            {
                if (testPort.BytesToRead > 0)
                {
                    byte[] incoming = new byte[testPort.BytesToRead];
                    int read = testPort.Read(incoming, 0, incoming.Length);
                    if (incoming.Any(b => (b & 0x80) != 0)) return true;
                }
                System.Threading.Thread.Sleep(50);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public void Open(string comport)
    {
        try
        {
            Close();
            _serialPort = new SerialPort(comport, 9600, Parity.None, 8, StopBits.One);
            _serialPort.ReadTimeout = 1500;
            _serialPort.WriteTimeout = 1500;
            
            // Set DTR/RTS before opening
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;
            
            _serialPort.Open();
            
            // Wait for board to stabilize after opening
            System.Threading.Thread.Sleep(2000);

            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            ConState = _serialPort.IsOpen;
            if (ConState)
            {
                _lastBytes = null; // Clear cache for new connection
                _buffer.Clear();
                _serialPort.DataReceived += SerialPort_DataReceived;
                
                // Request full board dump first
                Debug.WriteLine("[DGT] Requesting initial board dump (0x42)");
                _serialPort.Write(new byte[] { 0x42 }, 0, 1);
                
                // Give board more time to process the dump request before sending the next command
                System.Threading.Thread.Sleep(500);

                // Set board to "Send Changes" mode so it reports moves immediately
                Debug.WriteLine("[DGT] Setting board to send changes mode (0x41)");
                _serialPort.Write(new byte[] { 0x41 }, 0, 1);

                // Give it another moment to settle
                System.Threading.Thread.Sleep(250);

                _timer = new Timer();
                _timer.Interval = 250; // More frequent polling (4Hz) for better responsiveness

                _timer.Elapsed += Timer_Elapsed;
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error opening port {comport}: {e.Message}");
            ConState = false;
        }
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            if (_serialPort is not null && _serialPort.IsOpen)
            {
                ConState = true;
                _serialPort.Write(new byte[] { 0x42 }, 0, 1);
            }
            else
            {
                ConState = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Timer error: {ex.Message}");
            ConState = false;
        }
    }

    public void Close()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        if (_serialPort is not null)
        {
            ConState = false;
            if (_serialPort.IsOpen)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
            _serialPort.Dispose();
            _serialPort = null;
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort is not null && _serialPort.IsOpen)
            {
                int toRead = _serialPort.BytesToRead;
                if (toRead == 0) return;
                
                byte[] incoming = new byte[toRead];
                _serialPort.Read(incoming, 0, toRead);
                
                // Debug logging (only on large chunks or for debugging)
                // Debug.WriteLine($"[DGT] Received {toRead} bytes: {BitConverter.ToString(incoming)}");
                
                lock (_buffer)
                {
                    _buffer.AddRange(incoming);
                    
                    while (_buffer.Count >= 3)
                    {
                        // Check for DGT header (MSB set)
                        if ((_buffer[0] & 0x80) != 0)
                        {
                            int len = ((_buffer[1] & 0x7F) << 7) | (_buffer[2] & 0x7F);
                            
                            // Sanity check for DGT protocol lengths
                            if (len < 3 || len > 100)
                            {
                                Debug.WriteLine($"[DGT] Invalid length {len} for header 0x{_buffer[0]:X2}. Skipping.");
                                _buffer.RemoveAt(0);
                                continue;
                            }

                            if (_buffer.Count >= len)
                            {
                                byte[] msg = _buffer.GetRange(0, len).ToArray();
                                _buffer.RemoveRange(0, len);
                                
                                Debug.WriteLine($"[DGT] Processing message 0x{msg[0]:X2}, length {len}");
                                ProcessMessage(msg);
                            }
                            else break;
                        }
                        else
                        {
                            // Skip invalid data until MSB set
                            Debug.WriteLine($"[DGT] Skipping byte 0x{_buffer[0]:X2} (no MSB set)");
                            _buffer.RemoveAt(0);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"[DGT] SerialPort_DataReceived Error: {exception.Message}");
        }
    }

    private void ProcessMessage(byte[] msg)
    {
        if ((msg[0] == 0xC2 || msg[0] == 0x86) && msg.Length >= 67)
        {
            if (_lastBytes is null || !_lastBytes.SequenceEqual(msg))
            {
                Debug.WriteLine($"[DGT] Processing full board dump (0x{msg[0]:X2})");
                // Detect move before updating our local cache
                Figur? removedFigur = RemoveFromBoard(msg);
                
                _lastBytes = (byte[])msg.Clone();
                UpdateBoard?.Invoke(_lastBytes);

                if (removedFigur != null && MoveHelp)
                {
                    PossibleMoves = BoardHelp.CheckPossibleMoves(removedFigur);
                }
            }
            return; // Correctly handle 0x86/0xC2 and exit
        }
        else if (msg[0] == 0x8E && msg.Length >= 5)
        {
            // Field update: Byte 3 = Square (0-63), Byte 4 = Piece (0-12)
            Debug.WriteLine($"[DGT] Processing field update (0x8E): Square {msg[3]}, Piece {msg[4]}");
            if (_lastBytes != null && _lastBytes.Length >= 67)
            {
                int squareIdx = msg[3];
                byte pieceCode = msg[4];
                
                if (squareIdx >= 0 && squareIdx < 64)
                {
                    // Detect if a piece was removed for MoveHelp
                    if (pieceCode == 0 && _lastBytes[3 + squareIdx] != 0 && MoveHelp)
                    {
                        Figur? removedFigur = BoardHelp.GetFigurAndPos(_lastBytes[3 + squareIdx], 3 + squareIdx, FigurColor);
                        if (removedFigur != null)
                        {
                            PossibleMoves = BoardHelp.CheckPossibleMoves(removedFigur);
                        }
                    }

                    // Update our cached board state (Dump format has header, len1, len2 followed by 64 fields)
                    _lastBytes[3 + squareIdx] = pieceCode;
                    
                    // Notify UI with the updated full state
                    UpdateBoard?.Invoke((byte[])_lastBytes.Clone());
                }
            }
            else
            {
                // If we don't have a dump yet, request one
                Debug.WriteLine("[DGT] Received 0x8E without prior dump, requesting 0x42");
                try { _serialPort?.Write(new byte[] { 0x42 }, 0, 1); } catch { }
            }
            return; // Correctly handle 0x8E and exit
        }
        
        // Only log other message types that are not handled above
        Debug.WriteLine($"[DGT] Received other message: 0x{msg[0]:X2}, length {msg.Length}");
    }

    private Figur? RemoveFromBoard(byte[] bytes)
    {
        if (_lastBytes != null)
        {
            for (int i = 3; i < bytes.Length; i++)
            {
                if (bytes[i] != _lastBytes[i])
                    return BoardHelp.GetFigurAndPos(_lastBytes[i], i, FigurColor);
            }
        }

        return null;
    }

    private void ConvertToBoard(byte[] data)
    {
        int spalte = 1;

        Debug.WriteLine("");
        Debug.WriteLine("");

        for (int i = 3; i < data.Length; i++)
        {
            //Debug.Write(BoardHelp.GetFigur(data[i]));
            if (spalte == 8)
            {
                Debug.WriteLine("");
                spalte = 1;
            }
            else
                spalte++;
        }
    }
}