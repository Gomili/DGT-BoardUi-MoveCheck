using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace DgtUsbChessService
{
    internal static class BoardHelp
    {
        internal static Figur GetFigurAndPos(byte figurNr, int i, FigurColor color)
        {
            return new Figur(figurNr, GetPosFromInt(i), color);
        }

        internal static string FigurPositionToText(Figur figur)
        {
            string pos = string.Empty;

            switch (figur.Position.Y)
            {
                case 1: pos = "A"; break;
                case 2: pos = "B"; break;
                case 3: pos = "C"; break;
                case 4: pos = "D"; break;
                case 5: pos = "E"; break;
                case 6: pos = "F"; break;
                case 7: pos = "G"; break;
                case 8: pos = "H"; break;
            }

            pos += figur.Position.X;

            return pos;
        }

        private static Point GetPosFromInt(int i)
        {
            i = i - 3;
            switch (i)
            {
                case 0: return new Point(1, 8);
                case 1: return new Point(2, 8);
                case 2: return new Point(3, 8);
                case 3: return new Point(4, 8);
                case 4: return new Point(5, 8);
                case 5: return new Point(6, 8);
                case 6: return new Point(7, 8);
                case 7: return new Point(8, 8);

                case 8: return new Point(1, 7);
                case 9: return new Point(2, 7);
                case 10: return new Point(3, 7);
                case 11: return new Point(4, 7);
                case 12: return new Point(5, 7);
                case 13: return new Point(6, 7);
                case 14: return new Point(7, 7);
                case 15: return new Point(8, 7);

                case 16: return new Point(1, 6);
                case 17: return new Point(2, 6);
                case 18: return new Point(3, 6);
                case 19: return new Point(4, 6);
                case 20: return new Point(5, 6);
                case 21: return new Point(6, 6);
                case 22: return new Point(7, 6);
                case 23: return new Point(8, 6);

                case 24: return new Point(1, 5);
                case 25: return new Point(2, 5);
                case 26: return new Point(3, 5);
                case 27: return new Point(4, 5);
                case 28: return new Point(5, 5);
                case 29: return new Point(6, 5);
                case 30: return new Point(7, 5);
                case 31: return new Point(8, 5);

                case 32: return new Point(1, 4);
                case 33: return new Point(2, 4);
                case 34: return new Point(3, 4);
                case 35: return new Point(4, 4);
                case 36: return new Point(5, 4);
                case 37: return new Point(6, 4);
                case 38: return new Point(7, 4);
                case 39: return new Point(8, 4);

                case 40: return new Point(1, 3);
                case 41: return new Point(2, 3);
                case 42: return new Point(3, 3);
                case 43: return new Point(4, 3);
                case 44: return new Point(5, 3);
                case 45: return new Point(6, 3);
                case 46: return new Point(7, 3);
                case 47: return new Point(8, 3);

                case 48: return new Point(1, 2);
                case 49: return new Point(2, 2);
                case 50: return new Point(3, 2);
                case 51: return new Point(4, 2);
                case 52: return new Point(5, 2);
                case 53: return new Point(6, 2);
                case 54: return new Point(7, 2);
                case 55: return new Point(8, 2);

                case 56: return new Point(1, 1);
                case 57: return new Point(2, 1);
                case 58: return new Point(3, 1);
                case 59: return new Point(4, 1);
                case 60: return new Point(5, 1);
                case 61: return new Point(6, 1);
                case 62: return new Point(7, 1);
                case 63: return new Point(8, 1);
            }

            throw new Exception("No Position!");
        }

        //internal static Figur GetFigur(byte kennung)
        //{
        //    switch ((Figuren)kennung)
        //    {
        //        case Figuren.Frei:
        //            return "**";
        //        case Figuren.WBauer:
        //            return "WB";
        //        case Figuren.WTurm:
        //            return "WT";
        //        case Figuren.WSpringer:
        //            return "WS";
        //        case Figuren.WLaeufer:
        //            return "WL";
        //        case Figuren.WDame:
        //            return "WD";
        //        case Figuren.WKoenig:
        //            return "WK";
        //        case Figuren.SBauer:
        //            return "SB";
        //        case Figuren.STurm:
        //            return "ST";
        //        case Figuren.SSpringer:
        //            return "SS";
        //        case Figuren.SLaeufer:
        //            return "SL";
        //        case Figuren.SDame:
        //            return "SD";
        //        case Figuren.SKoenig:
        //            return "SK";
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(kennung), kennung, null);
        //    }
        //}

        public static List<string> CheckPossibleMoves(Figur figur)
        {

            return new List<string>();
        }
    }
}
