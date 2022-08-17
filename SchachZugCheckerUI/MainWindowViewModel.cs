using System;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchachZugCheckerUI.Model;
using Test;

namespace SchachZugCheckerUI;

public partial class MainWindowViewModel : ObservableObject
{
    #region BoardPropertys

    [ObservableProperty] private VisualChessField _a1 = new();
    [ObservableProperty] private VisualChessField _a2 = new();
    [ObservableProperty] private VisualChessField _a3 = new();
    [ObservableProperty] private VisualChessField _a4 = new();
    [ObservableProperty] private VisualChessField _a5 = new();
    [ObservableProperty] private VisualChessField _a6 = new();
    [ObservableProperty] private VisualChessField _a7 = new();
    [ObservableProperty] private VisualChessField _a8 = new();
    [ObservableProperty] private VisualChessField _b1 = new();
    [ObservableProperty] private VisualChessField _b2 = new();
    [ObservableProperty] private VisualChessField _b3 = new();
    [ObservableProperty] private VisualChessField _b4 = new();
    [ObservableProperty] private VisualChessField _b5 = new();
    [ObservableProperty] private VisualChessField _b6 = new();
    [ObservableProperty] private VisualChessField _b7 = new();
    [ObservableProperty] private VisualChessField _b8 = new();
    [ObservableProperty] private VisualChessField _c1 = new();
    [ObservableProperty] private VisualChessField _c2 = new();
    [ObservableProperty] private VisualChessField _c3 = new();
    [ObservableProperty] private VisualChessField _c4 = new();
    [ObservableProperty] private VisualChessField _c5 = new();
    [ObservableProperty] private VisualChessField _c6 = new();
    [ObservableProperty] private VisualChessField _c7 = new();
    [ObservableProperty] private VisualChessField _c8 = new();
    [ObservableProperty] private VisualChessField _d1 = new();
    [ObservableProperty] private VisualChessField _d2 = new();
    [ObservableProperty] private VisualChessField _d3 = new();
    [ObservableProperty] private VisualChessField _d4 = new();
    [ObservableProperty] private VisualChessField _d5 = new();
    [ObservableProperty] private VisualChessField _d6 = new();
    [ObservableProperty] private VisualChessField _d7 = new();
    [ObservableProperty] private VisualChessField _d8 = new();
    [ObservableProperty] private VisualChessField _e1 = new();
    [ObservableProperty] private VisualChessField _e2 = new();
    [ObservableProperty] private VisualChessField _e3 = new();
    [ObservableProperty] private VisualChessField _e4 = new();
    [ObservableProperty] private VisualChessField _e5 = new();
    [ObservableProperty] private VisualChessField _e6 = new();
    [ObservableProperty] private VisualChessField _e7 = new();
    [ObservableProperty] private VisualChessField _e8 = new();
    [ObservableProperty] private VisualChessField _f1 = new();
    [ObservableProperty] private VisualChessField _f2 = new();
    [ObservableProperty] private VisualChessField _f3 = new();
    [ObservableProperty] private VisualChessField _f4 = new();
    [ObservableProperty] private VisualChessField _f5 = new();
    [ObservableProperty] private VisualChessField _f6 = new();
    [ObservableProperty] private VisualChessField _f7 = new();
    [ObservableProperty] private VisualChessField _f8 = new();
    [ObservableProperty] private VisualChessField _g1 = new();
    [ObservableProperty] private VisualChessField _g2 = new();
    [ObservableProperty] private VisualChessField _g3 = new();
    [ObservableProperty] private VisualChessField _g4 = new();
    [ObservableProperty] private VisualChessField _g5 = new();
    [ObservableProperty] private VisualChessField _g6 = new();
    [ObservableProperty] private VisualChessField _g7 = new();
    [ObservableProperty] private VisualChessField _g8 = new();
    [ObservableProperty] private VisualChessField _h1 = new();
    [ObservableProperty] private VisualChessField _h2 = new();
    [ObservableProperty] private VisualChessField _h3 = new();
    [ObservableProperty] private VisualChessField _h4 = new();
    [ObservableProperty] private VisualChessField _h5 = new();
    [ObservableProperty] private VisualChessField _h6 = new();
    [ObservableProperty] private VisualChessField _h7 = new();
    [ObservableProperty] private VisualChessField _h8 = new();

    #endregion

    [ObservableProperty] private Visibility _closeVisi = Visibility.Hidden;
    [ObservableProperty] private Visibility _openVisi = Visibility.Visible;
    [ObservableProperty] private Brush _statusColorBrush = new SolidColorBrush(Colors.DarkRed);
    [ObservableProperty] private bool _textBoxEnable = true;
    [ObservableProperty] private string _comport = "COM3";
    
    public MainWindowViewModel()
    {
        ResetBoard();
    }

    private DgtSerDriver? DgtSerDriver { get; set; }

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
    private void Open()
    {
        DgtSerDriver = new DgtSerDriver();
        DgtSerDriver.Open(Comport);
        if (DgtSerDriver.ConState)
        {
            StatusColorBrush = new SolidColorBrush(Colors.Green);
            TextBoxEnable = false;
            OpenVisi = Visibility.Hidden;
            CloseVisi = Visibility.Visible;
            DgtSerDriver.UpdateBoard += DgtSerDriverOnUpdateBoard;
        }
        else
        {
            MessageBox.Show("Connection cannot be opened! Please use another port.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Close()
    {
        if (DgtSerDriver != null)
        {
            StatusColorBrush = new SolidColorBrush(Colors.DarkRed);
            TextBoxEnable = true;
            DgtSerDriver.Close();
            OpenVisi = Visibility.Visible;
            CloseVisi = Visibility.Hidden;

            DgtSerDriver.UpdateBoard -= DgtSerDriverOnUpdateBoard;
        }
    }

    private void DgtSerDriverOnUpdateBoard(byte[] data)
    {
        UpdateBoard(data);
    }

    public void UpdateBoard(byte[] data)
    {
        A8.Figur = GetFigur((Figuren)data[3]);
        B8.Figur = GetFigur((Figuren)data[4]);
        C8.Figur = GetFigur((Figuren)data[5]);
        D8.Figur = GetFigur((Figuren)data[6]);
        E8.Figur = GetFigur((Figuren)data[7]);
        F8.Figur = GetFigur((Figuren)data[8]);
        G8.Figur = GetFigur((Figuren)data[9]);
        H8.Figur = GetFigur((Figuren)data[10]);

        A7.Figur = GetFigur((Figuren)data[11]);
        B7.Figur = GetFigur((Figuren)data[12]);
        C7.Figur = GetFigur((Figuren)data[13]);
        D7.Figur = GetFigur((Figuren)data[14]);
        E7.Figur = GetFigur((Figuren)data[15]);
        F7.Figur = GetFigur((Figuren)data[16]);
        G7.Figur = GetFigur((Figuren)data[17]);
        H7.Figur = GetFigur((Figuren)data[18]);

        A6.Figur = GetFigur((Figuren)data[19]);
        B6.Figur = GetFigur((Figuren)data[20]);
        C6.Figur = GetFigur((Figuren)data[21]);
        D6.Figur = GetFigur((Figuren)data[22]);
        E6.Figur = GetFigur((Figuren)data[23]);
        F6.Figur = GetFigur((Figuren)data[24]);
        G6.Figur = GetFigur((Figuren)data[25]);
        H6.Figur = GetFigur((Figuren)data[26]);

        A5.Figur = GetFigur((Figuren)data[27]);
        B5.Figur = GetFigur((Figuren)data[28]);
        C5.Figur = GetFigur((Figuren)data[29]);
        D5.Figur = GetFigur((Figuren)data[30]);
        E5.Figur = GetFigur((Figuren)data[31]);
        F5.Figur = GetFigur((Figuren)data[32]);
        G5.Figur = GetFigur((Figuren)data[33]);
        H5.Figur = GetFigur((Figuren)data[34]);

        A4.Figur = GetFigur((Figuren)data[35]);
        B4.Figur = GetFigur((Figuren)data[36]);
        C4.Figur = GetFigur((Figuren)data[37]);
        D4.Figur = GetFigur((Figuren)data[38]);
        E4.Figur = GetFigur((Figuren)data[39]);
        F4.Figur = GetFigur((Figuren)data[40]);
        G4.Figur = GetFigur((Figuren)data[41]);
        H4.Figur = GetFigur((Figuren)data[42]);

        A3.Figur = GetFigur((Figuren)data[43]);
        B3.Figur = GetFigur((Figuren)data[44]);
        C3.Figur = GetFigur((Figuren)data[45]);
        D3.Figur = GetFigur((Figuren)data[46]);
        E3.Figur = GetFigur((Figuren)data[47]);
        F3.Figur = GetFigur((Figuren)data[48]);
        G3.Figur = GetFigur((Figuren)data[49]);
        H3.Figur = GetFigur((Figuren)data[50]);

        A2.Figur = GetFigur((Figuren)data[51]);
        B2.Figur = GetFigur((Figuren)data[52]);
        C2.Figur = GetFigur((Figuren)data[53]);
        D2.Figur = GetFigur((Figuren)data[54]);
        E2.Figur = GetFigur((Figuren)data[55]);
        F2.Figur = GetFigur((Figuren)data[56]);
        G2.Figur = GetFigur((Figuren)data[57]);
        H2.Figur = GetFigur((Figuren)data[58]);

        A1.Figur = GetFigur((Figuren)data[59]);
        B1.Figur = GetFigur((Figuren)data[60]);
        C1.Figur = GetFigur((Figuren)data[61]);
        D1.Figur = GetFigur((Figuren)data[62]);
        E1.Figur = GetFigur((Figuren)data[63]);
        F1.Figur = GetFigur((Figuren)data[64]);
        G1.Figur = GetFigur((Figuren)data[65]);
        H1.Figur = GetFigur((Figuren)data[66]);
    }

    private string? GetFigur(Figuren figur)
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
                throw new ArgumentOutOfRangeException(nameof(figur), figur, null);
        }
    }

}