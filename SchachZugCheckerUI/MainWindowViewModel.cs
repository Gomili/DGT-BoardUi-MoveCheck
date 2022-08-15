using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Test;

namespace SchachZugCheckerUI
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private string? _a1;
        private string? _a2;
        private string? _a3;
        private string? _a4;
        private string? _a5;
        private string? _a6;
        private string? _a7;
        private string? _a8;
        private string? _b1;
        private string? _b2;
        private string? _b3;
        private string? _b4;
        private string? _b5;
        private string? _b6;
        private string? _b7;
        private string? _b8;
        private string? _c1;
        private string? _c2;
        private string? _c3;
        private string? _c4;
        private string? _c5;
        private string? _c6;
        private string? _c7;
        private string? _c8;
        private string? _d1;
        private string? _d2;
        private string? _d3;
        private string? _d4;
        private string? _d5;
        private string? _d6;
        private string? _d7;
        private string? _d8;
        private string? _e1;
        private string? _e2;
        private string? _e3;
        private string? _e4;
        private string? _e5;
        private string? _e6;
        private string? _e7;
        private string? _e8;
        private string? _f1;
        private string? _f2;
        private string? _f3;
        private string? _f4;
        private string? _f5;
        private string? _f6;
        private string? _f7;
        private string? _f8;
        private string? _g1;
        private string? _g2;
        private string? _g3;
        private string? _g4;
        private string? _g5;
        private string? _g6;
        private string? _g7;
        private string? _g8;
        private string? _h1;
        private string? _h2;
        private string? _h3;
        private string? _h4;
        private string? _h5;
        private string? _h6;
        private string? _h7;
        private string? _h8;
        private Visibility _openVisi = Visibility.Visible;
        private Visibility _closeVisi = Visibility.Hidden;
        
        public DgtSerDriver? DgtSerDriver { get; set; }

        public string? A1
        {
            get => _a1;
            set
            {
                if (value == _a1) return;
                _a1 = value;
                OnPropertyChanged();
            }
        }
        public string? A2
        {
            get => _a2;
            set
            {
                if (value == _a2) return;
                _a2 = value;
                OnPropertyChanged();
            }
        }
        public string? A3
        {
            get => _a3;
            set
            {
                if (value == _a3) return;
                _a3 = value;
                OnPropertyChanged();
            }
        }
        public string? A4
        {
            get => _a4;
            set
            {
                if (value == _a4) return;
                _a4 = value;
                OnPropertyChanged();
            }
        }
        public string? A5
        {
            get => _a5;
            set
            {
                if (value == _a5) return;
                _a5 = value;
                OnPropertyChanged();
            }
        }
        public string? A6
        {
            get => _a6;
            set
            {
                if (value == _a6) return;
                _a6 = value;
                OnPropertyChanged();
            }
        }
        public string? A7
        {
            get => _a7;
            set
            {
                if (value == _a7) return;
                _a7 = value;
                OnPropertyChanged();
            }
        }
        public string? A8
        {
            get => _a8;
            set
            {
                if (value == _a8) return;
                _a8 = value;
                OnPropertyChanged();
            }
        }

        public string? B1
        {
            get => _b1;
            set
            {
                if (value == _b1) return;
                _b1 = value;
                OnPropertyChanged();
            }
        }
        public string? B2
        {
            get => _b2;
            set
            {
                if (value == _b2) return;
                _b2 = value;
                OnPropertyChanged();
            }
        }
        public string? B3
        {
            get => _b3;
            set
            {
                if (value == _b3) return;
                _b3 = value;
                OnPropertyChanged();
            }
        }
        public string? B4
        {
            get => _b4;
            set
            {
                if (value == _b4) return;
                _b4 = value;
                OnPropertyChanged();
            }
        }
        public string? B5
        {
            get => _b5;
            set
            {
                if (value == _b5) return;
                _b5 = value;
                OnPropertyChanged();
            }
        }
        public string? B6
        {
            get => _b6;
            set
            {
                if (value == _b6) return;
                _b6 = value;
                OnPropertyChanged();
            }
        }
        public string? B7
        {
            get => _b7;
            set
            {
                if (value == _b7) return;
                _b7 = value;
                OnPropertyChanged();
            }
        }
        public string? B8
        {
            get => _b8;
            set
            {
                if (value == _b8) return;
                _b8 = value;
                OnPropertyChanged();
            }
        }

        public string? C1
        {
            get => _c1;
            set
            {
                if (value == _c1) return;
                _c1 = value;
                OnPropertyChanged();
            }
        }
        public string? C2
        {
            get => _c2;
            set
            {
                if (value == _c2) return;
                _c2 = value;
                OnPropertyChanged();
            }
        }
        public string? C3
        {
            get => _c3;
            set
            {
                if (value == _c3) return;
                _c3 = value;
                OnPropertyChanged();
            }
        }
        public string? C4
        {
            get => _c4;
            set
            {
                if (value == _c4) return;
                _c4 = value;
                OnPropertyChanged();
            }
        }
        public string? C5
        {
            get => _c5;
            set
            {
                if (value == _c5) return;
                _c5 = value;
                OnPropertyChanged();
            }
        }
        public string? C6
        {
            get => _c6;
            set
            {
                if (value == _c6) return;
                _c6 = value;
                OnPropertyChanged();
            }
        }
        public string? C7
        {
            get => _c7;
            set
            {
                if (value == _c7) return;
                _c7 = value;
                OnPropertyChanged();
            }
        }
        public string? C8
        {
            get => _c8;
            set
            {
                if (value == _c8) return;
                _c8 = value;
                OnPropertyChanged();
            }
        }

        public string? D1
        {
            get => _d1;
            set
            {
                if (value == _d1) return;
                _d1 = value;
                OnPropertyChanged();
            }
        }
        public string? D2
        {
            get => _d2;
            set
            {
                if (value == _d2) return;
                _d2 = value;
                OnPropertyChanged();
            }
        }
        public string? D3
        {
            get => _d3;
            set
            {
                if (value == _d3) return;
                _d3 = value;
                OnPropertyChanged();
            }
        }
        public string? D4
        {
            get => _d4;
            set
            {
                if (value == _d4) return;
                _d4 = value;
                OnPropertyChanged();
            }
        }
        public string? D5
        {
            get => _d5;
            set
            {
                if (value == _d5) return;
                _d5 = value;
                OnPropertyChanged();
            }
        }
        public string? D6
        {
            get => _d6;
            set
            {
                if (value == _d6) return;
                _d6 = value;
                OnPropertyChanged();
            }
        }
        public string? D7
        {
            get => _d7;
            set
            {
                if (value == _d7) return;
                _d7 = value;
                OnPropertyChanged();
            }
        }
        public string? D8
        {
            get => _d8;
            set
            {
                if (value == _d8) return;
                _d8 = value;
                OnPropertyChanged();
            }
        }

        public string? E1
        {
            get => _e1;
            set
            {
                if (value == _e1) return;
                _e1 = value;
                OnPropertyChanged();
            }
        }
        public string? E2
        {
            get => _e2;
            set
            {
                if (value == _e2) return;
                _e2 = value;
                OnPropertyChanged();
            }
        }
        public string? E3
        {
            get => _e3;
            set
            {
                if (value == _e3) return;
                _e3 = value;
                OnPropertyChanged();
            }
        }
        public string? E4
        {
            get => _e4;
            set
            {
                if (value == _e4) return;
                _e4 = value;
                OnPropertyChanged();
            }
        }
        public string? E5
        {
            get => _e5;
            set
            {
                if (value == _e5) return;
                _e5 = value;
                OnPropertyChanged();
            }
        }
        public string? E6
        {
            get => _e6;
            set
            {
                if (value == _e6) return;
                _e6 = value;
                OnPropertyChanged();
            }
        }
        public string? E7
        {
            get => _e7;
            set
            {
                if (value == _e7) return;
                _e7 = value;
                OnPropertyChanged();
            }
        }
        public string? E8
        {
            get => _e8;
            set
            {
                if (value == _e8) return;
                _e8 = value;
                OnPropertyChanged();
            }
        }

        public string? F1
        {
            get => _f1;
            set
            {
                if (value == _f1) return;
                _f1 = value;
                OnPropertyChanged();
            }
        }
        public string? F2
        {
            get => _f2;
            set
            {
                if (value == _f2) return;
                _f2 = value;
                OnPropertyChanged();
            }
        }
        public string? F3
        {
            get => _f3;
            set
            {
                if (value == _f3) return;
                _f3 = value;
                OnPropertyChanged();
            }
        }
        public string? F4
        {
            get => _f4;
            set
            {
                if (value == _f4) return;
                _f4 = value;
                OnPropertyChanged();
            }
        }
        public string? F5
        {
            get => _f5;
            set
            {
                if (value == _f5) return;
                _f5 = value;
                OnPropertyChanged();
            }
        }
        public string? F6
        {
            get => _f6;
            set
            {
                if (value == _f6) return;
                _f6 = value;
                OnPropertyChanged();
            }
        }
        public string? F7
        {
            get => _f7;
            set
            {
                if (value == _f7) return;
                _f7 = value;
                OnPropertyChanged();
            }
        }
        public string? F8
        {
            get => _f8;
            set
            {
                if (value == _f8) return;
                _f8 = value;
                OnPropertyChanged();
            }
        }
        
        public string? G1
        {
            get => _g1;
            set
            {
                if (value == _g1) return;
                _g1 = value;
                OnPropertyChanged();
            }
        }
        public string? G2
        {
            get => _g2;
            set
            {
                if (value == _g2) return;
                _g2 = value;
                OnPropertyChanged();
            }
        }
        public string? G3
        {
            get => _g3;
            set
            {
                if (value == _g3) return;
                _g3 = value;
                OnPropertyChanged();
            }
        }
        public string? G4
        {
            get => _g4;
            set
            {
                if (value == _g4) return;
                _g4 = value;
                OnPropertyChanged();
            }
        }
        public string? G5
        {
            get => _g5;
            set
            {
                if (value == _g5) return;
                _g5 = value;
                OnPropertyChanged();
            }
        }
        public string? G6
        {
            get => _g6;
            set
            {
                if (value == _g6) return;
                _g6 = value;
                OnPropertyChanged();
            }
        }
        public string? G7
        {
            get => _g7;
            set
            {
                if (value == _g7) return;
                _g7 = value;
                OnPropertyChanged();
            }
        }
        public string? G8
        {
            get => _g8;
            set
            {
                if (value == _g8) return;
                _g8 = value;
                OnPropertyChanged();
            }
        }

        public string? H1
        {
            get => _h1;
            set
            {
                if (value == _h1) return;
                _h1 = value;
                OnPropertyChanged();
            }
        }
        public string? H2
        {
            get => _h2;
            set
            {
                if (value == _h2) return;
                _h2 = value;
                OnPropertyChanged();
            }
        }
        public string? H3
        {
            get => _h3;
            set
            {
                if (value == _h3) return;
                _h3 = value;
                OnPropertyChanged();
            }
        }
        public string? H4
        {
            get => _h4;
            set
            {
                if (value == _h4) return;
                _h4 = value;
                OnPropertyChanged();
            }
        }
        public string? H5
        {
            get => _h5;
            set
            {
                if (value == _h5) return;
                _h5 = value;
                OnPropertyChanged();
            }
        }
        public string? H6
        {
            get => _h6;
            set
            {
                if (value == _h6) return;
                _h6 = value;
                OnPropertyChanged();
            }
        }
        public string? H7
        {
            get => _h7;
            set
            {
                if (value == _h7) return;
                _h7 = value;
                OnPropertyChanged();
            }
        }
        public string? H8
        {
            get => _h8;
            set
            {
                if (value == _h8) return;
                _h8 = value;
                OnPropertyChanged();
            }
        }

        public Visibility OpenVisi
        {
            get => _openVisi;
            set
            {
                if (value == _openVisi) return;
                _openVisi = value;
                OnPropertyChanged();
            }
        }

        public Visibility CloseVisi
        {
            get => _closeVisi;
            set
            {
                if (value == _closeVisi) return;
                _closeVisi = value;
                OnPropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
