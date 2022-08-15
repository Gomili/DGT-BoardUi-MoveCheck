using System.Diagnostics;
using System.IO.Ports;
using System.Text;
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

    public void Open()
    {
        _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        try
        {
            _serialPort.Open();
            _serialPort.DataReceived += SerialPort_DataReceived;
            var a = _serialPort.IsOpen;
            if (a)
            {
                _timer = new Timer();
                _timer.Interval = 300;

                _timer.Elapsed += Timer_Elapsed;
                _timer.AutoReset = true;
                _timer.Enabled = true;
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
        if (_serialPort.IsOpen)
            _serialPort?.Write(buffer, 0, 1);
    }

    public void Close()
    {
        if (_serialPort is not null)
        {
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
                    _lastBytes = bytes;
                    UpdateBoard?.Invoke(bytes);
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void ConvertToBoard(byte[] data)
    {
        int spalte = 1;

        Debug.WriteLine("");
        Debug.WriteLine("");

        for (int i = 3; i < data.Length; i++)
        {
            Debug.Write(GetFigur(data[i]));
            if (spalte == 8)
            {
                Debug.WriteLine("");
                spalte = 1;
            }
            else
                spalte++;
        }
    }

    private string GetFigur(byte kennung)
    {
        switch ((Figuren)kennung)
        {
            case Figuren.Frei:
                return "**";
            case Figuren.WBauer:
                return "WB";
            case Figuren.WTurm:
                return "WT";
            case Figuren.WSpringer:
                return "WS";
            case Figuren.WLaeufer:
                return "WL";
            case Figuren.WDame:
                return "WD";
            case Figuren.WKoenig:
                return "WK";
            case Figuren.SBauer:
                return "SB";
            case Figuren.STurm:
                return "ST";
            case Figuren.SSpringer:
                return "SS";
            case Figuren.SLaeufer:
                return "SL";
            case Figuren.SDame:
                return "SD";
            case Figuren.SKoenig:
                return "SK";
            default:
                throw new ArgumentOutOfRangeException(nameof(kennung), kennung, null);
        }
    }
}