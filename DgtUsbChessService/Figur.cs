using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DgtUsbChessService
{
    public enum FigurColor
    {
        White = 0,
        Black = 1,
    }
    
    public class Figur
    {

        public Figur(int id, Point pos, FigurColor color)
        {
            FigurId = id;
            Position = pos;
            FigurColor = color;
        }

        public int  FigurId { get; set; }
        public Point Position { get; set; }
        public FigurColor FigurColor { get; set; }
    }
}
