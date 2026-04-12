using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchachZugCheckerWinUI.Model;
using Test;

namespace SchachZugCheckerWinUI;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue;

    #region BoardPropertys
    [ObservableProperty] public partial VisualChessField A1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField A8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField B8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField C8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField D8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField E8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField F8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField G8 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H1 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H2 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H3 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H4 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H5 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H6 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H7 { get; set; } = new();
    [ObservableProperty] public partial VisualChessField H8 { get; set; } = new();
    #endregion

    [ObservableProperty] public partial Visibility CloseVisi { get; set; } = Visibility.Collapsed;
    [ObservableProperty] public partial Visibility OpenVisi { get; set; } = Visibility.Visible;
    [ObservableProperty] public partial Brush StatusColorBrush { get; set; } = new SolidColorBrush(Colors.DarkRed);
    [ObservableProperty] public partial bool TextBoxEnable { get; set; } = true;
    [ObservableProperty] public partial string Comport { get; set; } = "COM3";
    [ObservableProperty] public partial bool MoveHelp { get; set; } = false;

    partial void OnMoveHelpChanged(bool value)
    {
        if (_dgtDriver is not null)
            _dgtDriver.MoveHelp = value;
    }
    
    private DgtSerDriver _dgtDriver { get; set; }

    public MainWindowViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        ResetBoard();
        _dgtDriver = new DgtSerDriver();
        _dgtDriver.UpdateBoard += DgtSerDriverOnUpdateBoard;
    }

    private void ResetBoard()
    {
        A1.Figur = Files.WT;
        B1.Figur = Files.WS;
        C1.Figur = Files.WL;
        D1.Figur = Files.WD;
        E1.Figur = Files.WK;
        F1.Figur = Files.WL;
        G1.Figur = Files.WS;
        H1.Figur = Files.WT;

        A2.Figur = Files.WB;
        B2.Figur = Files.WB;
        C2.Figur = Files.WB;
        D2.Figur = Files.WB;
        E2.Figur = Files.WB;
        F2.Figur = Files.WB;
        G2.Figur = Files.WB;
        H2.Figur = Files.WB;

        A8.Figur = Files.ST;
        B8.Figur = Files.SS;
        C8.Figur = Files.SL;
        D8.Figur = Files.SD;
        E8.Figur = Files.SK;
        F8.Figur = Files.SL;
        G8.Figur = Files.SS;
        H8.Figur = Files.ST;

        A7.Figur = Files.SB;
        B7.Figur = Files.SB;
        C7.Figur = Files.SB;
        D7.Figur = Files.SB;
        E7.Figur = Files.SB;
        F7.Figur = Files.SB;
        G7.Figur = Files.SB;
        H7.Figur = Files.SB;
    }

    [RelayCommand]
    private async Task Open()
    {
        TextBoxEnable = false;
        try
        {
            string[] ports = SerialPort.GetPortNames();
            string? foundPort = null;
            string currentCom = Comport?.Trim() ?? "";

            if (!string.IsNullOrEmpty(currentCom))
            {
                if (await Task.Run(() => DgtSerDriver.TestConnection(currentCom)))
                {
                    foundPort = currentCom;
                }
            }

            if (foundPort == null)
            {
                foreach (var port in ports)
                {
                    if (string.Equals(port, currentCom, StringComparison.OrdinalIgnoreCase)) continue;
                    if (await Task.Run(() => DgtSerDriver.TestConnection(port)))
                    {
                        foundPort = port;
                        Comport = port;
                        break;
                    }
                }
            }

            if (foundPort != null)
            {
                await Task.Delay(1000);

                _dgtDriver.Open(foundPort);
                if (_dgtDriver.ConState)
                {
                    StatusColorBrush = new SolidColorBrush(Colors.Green);
                    TextBoxEnable = false;
                    _dgtDriver.MoveHelp = MoveHelp;
                    OpenVisi = Visibility.Collapsed;
                    CloseVisi = Visibility.Visible;
                }
                else
                {
                    Debug.WriteLine($"Could not open connection to {foundPort}");
                    TextBoxEnable = true;
                }
            }
            else
            {
                Debug.WriteLine("No DGT Board found!");
                TextBoxEnable = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred during connection: {ex.Message}");
            TextBoxEnable = true;
        }
    }

    [RelayCommand]
    private void Close()
    {
        if (_dgtDriver != null)
        {
            StatusColorBrush = new SolidColorBrush(Colors.DarkRed);
            TextBoxEnable = true;
            _dgtDriver.Close();
            OpenVisi = Visibility.Visible;
            CloseVisi = Visibility.Collapsed;
        }
    }

    [RelayCommand]
    private void HelpSwitched(bool mode)
    {
        if (_dgtDriver is not null)
            _dgtDriver.MoveHelp = mode;
    }

    private void DgtSerDriverOnUpdateBoard(byte[] data)
    {
        Debug.WriteLine($"[ViewModel] Received Board Update, Length: {data.Length}");
        _dispatcherQueue.TryEnqueue(() => UpdateBoard(data));
    }

    public void UpdateBoard(byte[] data)
    {
        if (data.Length < 67) return; 
        
        UpdateField(A8, (Figuren)data[3]);
        UpdateField(B8, (Figuren)data[4]);
        UpdateField(C8, (Figuren)data[5]);
        UpdateField(D8, (Figuren)data[6]);
        UpdateField(E8, (Figuren)data[7]);
        UpdateField(F8, (Figuren)data[8]);
        UpdateField(G8, (Figuren)data[9]);
        UpdateField(H8, (Figuren)data[10]);

        UpdateField(A7, (Figuren)data[11]);
        UpdateField(B7, (Figuren)data[12]);
        UpdateField(C7, (Figuren)data[13]);
        UpdateField(D7, (Figuren)data[14]);
        UpdateField(E7, (Figuren)data[15]);
        UpdateField(F7, (Figuren)data[16]);
        UpdateField(G7, (Figuren)data[17]);
        UpdateField(H7, (Figuren)data[18]);

        UpdateField(A6, (Figuren)data[19]);
        UpdateField(B6, (Figuren)data[20]);
        UpdateField(C6, (Figuren)data[21]);
        UpdateField(D6, (Figuren)data[22]);
        UpdateField(E6, (Figuren)data[23]);
        UpdateField(F6, (Figuren)data[24]);
        UpdateField(G6, (Figuren)data[25]);
        UpdateField(H6, (Figuren)data[26]);

        UpdateField(A5, (Figuren)data[27]);
        UpdateField(B5, (Figuren)data[28]);
        UpdateField(C5, (Figuren)data[29]);
        UpdateField(D5, (Figuren)data[30]);
        UpdateField(E5, (Figuren)data[31]);
        UpdateField(F5, (Figuren)data[32]);
        UpdateField(G5, (Figuren)data[33]);
        UpdateField(H5, (Figuren)data[34]);

        UpdateField(A4, (Figuren)data[35]);
        UpdateField(B4, (Figuren)data[36]);
        UpdateField(C4, (Figuren)data[37]);
        UpdateField(D4, (Figuren)data[38]);
        UpdateField(E4, (Figuren)data[39]);
        UpdateField(F4, (Figuren)data[40]);
        UpdateField(G4, (Figuren)data[41]);
        UpdateField(H4, (Figuren)data[42]);

        UpdateField(A3, (Figuren)data[43]);
        UpdateField(B3, (Figuren)data[44]);
        UpdateField(C3, (Figuren)data[45]);
        UpdateField(D3, (Figuren)data[46]);
        UpdateField(E3, (Figuren)data[47]);
        UpdateField(F3, (Figuren)data[48]);
        UpdateField(G3, (Figuren)data[49]);
        UpdateField(H3, (Figuren)data[50]);

        UpdateField(A2, (Figuren)data[51]);
        UpdateField(B2, (Figuren)data[52]);
        UpdateField(C2, (Figuren)data[53]);
        UpdateField(D2, (Figuren)data[54]);
        UpdateField(E2, (Figuren)data[55]);
        UpdateField(F2, (Figuren)data[56]);
        UpdateField(G2, (Figuren)data[57]);
        UpdateField(H2, (Figuren)data[58]);

        UpdateField(A1, (Figuren)data[59]);
        UpdateField(B1, (Figuren)data[60]);
        UpdateField(C1, (Figuren)data[61]);
        UpdateField(D1, (Figuren)data[62]);
        UpdateField(E1, (Figuren)data[63]);
        UpdateField(F1, (Figuren)data[64]);
        UpdateField(G1, (Figuren)data[65]);
        UpdateField(H1, (Figuren)data[66]);
    }

    private void UpdateField(VisualChessField field, Figuren newFigur)
    {
        string? figurPath = GetFigur(newFigur);
        if (field.Figur != figurPath)
        {
            field.Figur = figurPath;
        }
    }

    private string? GetFigur(Figuren figur)
    {
        try
        {
            switch (figur)
            {
                case Figuren.Frei:
                    return Files.Leer;
                case Figuren.WBauer:
                    return Files.WB;
                case Figuren.WTurm:
                    return Files.WT;
                case Figuren.WSpringer:
                    return Files.WS;
                case Figuren.WLaeufer:
                    return Files.WL;
                case Figuren.WKoenig:
                    return Files.WK;
                case Figuren.WDame:
                    return Files.WD;
                case Figuren.SBauer:
                    return Files.SB;
                case Figuren.STurm:
                    return Files.ST;
                case Figuren.SSpringer:
                    return Files.SS;
                case Figuren.SLaeufer:
                    return Files.SL;
                case Figuren.SKoenig:
                    return Files.SK;
                case Figuren.SDame:
                    return Files.SD;
                default:
                    return Files.Leer;
            }
        }
        catch (Exception)
        {
            return Files.Leer;
        }
    }
}
