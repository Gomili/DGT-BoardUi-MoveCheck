using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
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

    public delegate void UpdateBoardEventHandler(byte[] data);

    public event UpdateBoardEventHandler UpdateBoard;

    public bool ConState { get; set; } = false;
    public bool MoveHelp { get; set; } = false;
    public FigurColor FigurColor { get; set; } = FigurColor.White;
    public List<string> Moves { get; set; } = new List<string>();
    public List<string> PossibleMoves { get; set; } = new List<string>();

    public void Open(string comport)
    {
        _serialPort = new SerialPort(comport, 9600, Parity.None, 8, StopBits.One);
        try
        {
            if (_serialPort != null)
            {
                _serialPort.Open();
                ConState = _serialPort.IsOpen;
                if (ConState)
                {
                    _serialPort.DataReceived += SerialPort_DataReceived;
                    _timer = new Timer();
                    _timer.Interval = 300;

                    _timer.Elapsed += Timer_Elapsed;
                    _timer.AutoReset = true;
                    _timer.Enabled = true;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var buffer = new byte[] { 0x42 };
        if (_serialPort is not null && _serialPort.IsOpen)
        {
            ConState = true;
            _serialPort?.Write(buffer, 0, 1);
        }
        else
            ConState = false;
    }

    public void Close()
    {
        if (_serialPort is not null)
        {
            ConState = false;
            _serialPort.Close();
            _serialPort.DataReceived -= SerialPort_DataReceived;
            _serialPort.Dispose();
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort is not null)
            {
                var ret = _serialPort.ReadExisting();
                var bytes = Encoding.ASCII.GetBytes(ret);
                if (_lastBytes is null || !_lastBytes.SequenceEqual(bytes))
                {
                    Figur? figur = RemoveFromBoard(bytes);
                    if (figur != null)
                    {
                        PossibleMoves = BoardHelp.CheckPossibleMoves(figur);
                        UpdateBoard?.Invoke(bytes);
                    }
                    _lastBytes = bytes;
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private Figur? RemoveFromBoard(byte[] bytes)
    {
        if (_lastBytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
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