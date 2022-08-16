using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Test;

namespace SchachZugCheckerUI
{
    public partial class MainWindowViewModel : ObservableObject
    {
        #region BoardPropertys
        
        [ObservableProperty] private string? _a1;
        [ObservableProperty] private string? _a2;
        [ObservableProperty] private string? _a3;
        [ObservableProperty] private string? _a4;
        [ObservableProperty] private string? _a5;
        [ObservableProperty] private string? _a6;
        [ObservableProperty] private string? _a7;
        [ObservableProperty] private string? _a8;
        [ObservableProperty] private string? _b1;
        [ObservableProperty] private string? _b2;
        [ObservableProperty] private string? _b3;
        [ObservableProperty] private string? _b4;
        [ObservableProperty] private string? _b5;
        [ObservableProperty] private string? _b6;
        [ObservableProperty] private string? _b7;
        [ObservableProperty] private string? _b8;
        [ObservableProperty] private string? _c1;
        [ObservableProperty] private string? _c2;
        [ObservableProperty] private string? _c3;
        [ObservableProperty] private string? _c4;
        [ObservableProperty] private string? _c5;
        [ObservableProperty] private string? _c6;
        [ObservableProperty] private string? _c7;
        [ObservableProperty] private string? _c8;
        [ObservableProperty] private string? _d1;
        [ObservableProperty] private string? _d2;
        [ObservableProperty] private string? _d3;
        [ObservableProperty] private string? _d4;
        [ObservableProperty] private string? _d5;
        [ObservableProperty] private string? _d6;
        [ObservableProperty] private string? _d7;
        [ObservableProperty] private string? _d8;
        [ObservableProperty] private string? _e1;
        [ObservableProperty] private string? _e2;
        [ObservableProperty] private string? _e3;
        [ObservableProperty] private string? _e4;
        [ObservableProperty] private string? _e5;
        [ObservableProperty] private string? _e6;
        [ObservableProperty] private string? _e7;
        [ObservableProperty] private string? _e8;
        [ObservableProperty] private string? _f1;
        [ObservableProperty] private string? _f2;
        [ObservableProperty] private string? _f3;
        [ObservableProperty] private string? _f4;
        [ObservableProperty] private string? _f5;
        [ObservableProperty] private string? _f6;
        [ObservableProperty] private string? _f7;
        [ObservableProperty] private string? _f8;
        [ObservableProperty] private string? _g1;
        [ObservableProperty] private string? _g2;
        [ObservableProperty] private string? _g3;
        [ObservableProperty] private string? _g4;
        [ObservableProperty] private string? _g5;
        [ObservableProperty] private string? _g6;
        [ObservableProperty] private string? _g7;
        [ObservableProperty] private string? _g8;
        [ObservableProperty] private string? _h1;
        [ObservableProperty] private string? _h2;
        [ObservableProperty] private string? _h3;
        [ObservableProperty] private string? _h4;
        [ObservableProperty] private string? _h5;
        [ObservableProperty] private string? _h6;
        [ObservableProperty] private string? _h7;
        [ObservableProperty] private string? _h8;
        #endregion
        
        [ObservableProperty] private Visibility _openVisi = Visibility.Visible;
        [ObservableProperty] private Visibility _closeVisi = Visibility.Hidden;
        
        public DgtSerDriver? DgtSerDriver { get; set; }

        public MainWindowViewModel()
        {
            ResetBoard();
        }

        private void ResetBoard()
        {
            A1 = Files.WT;
            B1 = Files.WS;
            C1 = Files.WL;
            D1 = Files.WD;
            E1 = Files.WK;
            F1 = Files.WL;
            G1 = Files.WS;
            H1 = Files.WT;

            A2 = Files.WB;
            B2 = Files.WB;
            C2 = Files.WB;
            D2 = Files.WB;
            E2 = Files.WB;
            F2 = Files.WB;
            G2 = Files.WB;
            H2 = Files.WB;

            A8 = Files.ST;
            B8 = Files.SS;
            C8 = Files.SL;
            D8 = Files.SD;
            E8 = Files.SK;
            F8 = Files.SL;
            G8 = Files.SS;
            H8 = Files.ST;

            A7 = Files.SB;
            B7 = Files.SB;
            C7 = Files.SB;
            D7 = Files.SB;
            E7 = Files.SB;
            F7 = Files.SB;
            G7 = Files.SB;
            H7 = Files.SB;
        }

        [RelayCommand]
        private void Open()
        {
            DgtSerDriver = new DgtSerDriver();
            DgtSerDriver.Open();
            OpenVisi = Visibility.Hidden;
            CloseVisi = Visibility.Visible;
            DgtSerDriver.UpdateBoard += DgtSerDriverOnUpdateBoard;
        }

        [RelayCommand]
        private void Close()
        {
            if (DgtSerDriver != null)
            {
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
            A8 = GetFigur((Figuren)data[3]);
            B8 = GetFigur((Figuren)data[4]);
            C8 = GetFigur((Figuren)data[5]);
            D8 = GetFigur((Figuren)data[6]);
            E8 = GetFigur((Figuren)data[7]);
            F8 = GetFigur((Figuren)data[8]);
            G8 = GetFigur((Figuren)data[9]);
            H8 = GetFigur((Figuren)data[10]);

            A7 = GetFigur((Figuren)data[11]);
            B7 = GetFigur((Figuren)data[12]);
            C7 = GetFigur((Figuren)data[13]);
            D7 = GetFigur((Figuren)data[14]);
            E7 = GetFigur((Figuren)data[15]);
            F7 = GetFigur((Figuren)data[16]);
            G7 = GetFigur((Figuren)data[17]);
            H7 = GetFigur((Figuren)data[18]);

            A6 = GetFigur((Figuren)data[19]);
            B6 = GetFigur((Figuren)data[20]);
            C6 = GetFigur((Figuren)data[21]);
            D6 = GetFigur((Figuren)data[22]);
            E6 = GetFigur((Figuren)data[23]);
            F6 = GetFigur((Figuren)data[24]);
            G6 = GetFigur((Figuren)data[25]);
            H6 = GetFigur((Figuren)data[26]);

            A5 = GetFigur((Figuren)data[27]);
            B5 = GetFigur((Figuren)data[28]);
            C5 = GetFigur((Figuren)data[29]);
            D5 = GetFigur((Figuren)data[30]);
            E5 = GetFigur((Figuren)data[31]);
            F5 = GetFigur((Figuren)data[32]);
            G5 = GetFigur((Figuren)data[33]);
            H5 = GetFigur((Figuren)data[34]);

            A4 = GetFigur((Figuren)data[35]);
            B4 = GetFigur((Figuren)data[36]);
            C4 = GetFigur((Figuren)data[37]);
            D4 = GetFigur((Figuren)data[38]);
            E4 = GetFigur((Figuren)data[39]);
            F4 = GetFigur((Figuren)data[40]);
            G4 = GetFigur((Figuren)data[41]);
            H4 = GetFigur((Figuren)data[42]);

            A3 = GetFigur((Figuren)data[43]);
            B3 = GetFigur((Figuren)data[44]);
            C3 = GetFigur((Figuren)data[45]);
            D3 = GetFigur((Figuren)data[46]);
            E3 = GetFigur((Figuren)data[47]);
            F3 = GetFigur((Figuren)data[48]);
            G3 = GetFigur((Figuren)data[49]);
            H3 = GetFigur((Figuren)data[50]);

            A2 = GetFigur((Figuren)data[51]);
            B2 = GetFigur((Figuren)data[52]);
            C2 = GetFigur((Figuren)data[53]);
            D2 = GetFigur((Figuren)data[54]);
            E2 = GetFigur((Figuren)data[55]);
            F2 = GetFigur((Figuren)data[56]);
            G2 = GetFigur((Figuren)data[57]);
            H2 = GetFigur((Figuren)data[58]);

            A1 = GetFigur((Figuren)data[59]);
            B1 = GetFigur((Figuren)data[60]);
            C1 = GetFigur((Figuren)data[61]);
            D1 = GetFigur((Figuren)data[62]);
            E1 = GetFigur((Figuren)data[63]);
            F1 = GetFigur((Figuren)data[64]);
            G1 = GetFigur((Figuren)data[65]);
            H1 = GetFigur((Figuren)data[66]);
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
}
