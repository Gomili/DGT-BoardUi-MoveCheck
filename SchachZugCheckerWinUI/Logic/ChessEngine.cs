using System;
using System.Collections.Generic;
using System.Linq;
using SchachZugCheckerWinUI.Model;

namespace SchachZugCheckerWinUI.Logic
{
    public class ChessEngine
    {
        public static List<string> GetLegalMoves(VisualChessField selectedField, IList<VisualChessField> allFields)
        {
            List<string> legalMoves = new List<string>();
            if (string.IsNullOrEmpty(selectedField.Figur) || selectedField.Figur.Contains("Leer"))
                return legalMoves;

            string figurPath = selectedField.Figur.ToLower();
            bool isWhite = figurPath.Contains("_w");
            int startRow = selectedField.Row;
            int startCol = selectedField.Column;

            // Helper to get field at position
            VisualChessField? GetField(int r, int c) => allFields.FirstOrDefault(f => f.Row == r && f.Column == c);

            // Directions for sliding pieces
            int[][] rookDirs = { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 } };
            int[][] bishopDirs = { new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 } };
            int[][] queenDirs = rookDirs.Concat(bishopDirs).ToArray();

            if (figurPath.Contains("_wb") || figurPath.Contains("_sb")) // Pawn
            {
                int dir = isWhite ? -1 : 1; // White moves up (decreasing row), Black down
                int startRowPawn = isWhite ? 6 : 1;

                // Move forward 1
                var f1 = GetField(startRow + dir, startCol);
                if (f1 != null && IsEmpty(f1))
                {
                    legalMoves.Add(f1.BoardPos);
                    // Move forward 2 from start
                    if (startRow == startRowPawn)
                    {
                        var f2 = GetField(startRow + 2 * dir, startCol);
                        if (f2 != null && IsEmpty(f2)) legalMoves.Add(f2.BoardPos);
                    }
                }

                // Captures
                int[] capCols = { startCol - 1, startCol + 1 };
                foreach (var c in capCols)
                {
                    var fc = GetField(startRow + dir, c);
                    if (fc != null && IsOpponent(fc, isWhite)) legalMoves.Add(fc.BoardPos);
                }
            }
            else if (figurPath.Contains("_wt") || figurPath.Contains("_st")) // Rook
            {
                AddSlidingMoves(legalMoves, startRow, startCol, rookDirs, isWhite, GetField);
            }
            else if (figurPath.Contains("_wl") || figurPath.Contains("_sl")) // Bishop
            {
                AddSlidingMoves(legalMoves, startRow, startCol, bishopDirs, isWhite, GetField);
            }
            else if (figurPath.Contains("_wd") || figurPath.Contains("_sd")) // Queen
            {
                AddSlidingMoves(legalMoves, startRow, startCol, queenDirs, isWhite, GetField);
            }
            else if (figurPath.Contains("_ws") || figurPath.Contains("_ss")) // Knight
            {
                int[][] knightMoves = { 
                    new[] { -2, -1 }, new[] { -2, 1 }, new[] { -1, -2 }, new[] { -1, 2 },
                    new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 } 
                };
                foreach (var m in knightMoves)
                {
                    var f = GetField(startRow + m[0], startCol + m[1]);
                    if (f != null && (IsEmpty(f) || IsOpponent(f, isWhite))) legalMoves.Add(f.BoardPos);
                }
            }
            else if (figurPath.Contains("_wk") || figurPath.Contains("_sk")) // King
            {
                foreach (var d in queenDirs)
                {
                    var f = GetField(startRow + d[0], startCol + d[1]);
                    if (f != null && (IsEmpty(f) || IsOpponent(f, isWhite))) legalMoves.Add(f.BoardPos);
                }
            }

            return legalMoves;
        }

        private static void AddSlidingMoves(List<string> legalMoves, int startRow, int startCol, int[][] dirs, bool isWhite, Func<int, int, VisualChessField?> getField)
        {
            foreach (var d in dirs)
            {
                for (int i = 1; i < 8; i++)
                {
                    int r = startRow + d[0] * i;
                    int c = startCol + d[1] * i;
                    var f = getField(r, c);
                    if (f == null) break;
                    
                    if (IsEmpty(f))
                    {
                        legalMoves.Add(f.BoardPos);
                    }
                    else
                    {
                        if (IsOpponent(f, isWhite)) legalMoves.Add(f.BoardPos);
                        break; // Blocked
                    }
                }
            }
        }

        private static bool IsEmpty(VisualChessField field) => string.IsNullOrEmpty(field.Figur) || field.Figur.Contains("Leer");
        private static bool IsOpponent(VisualChessField field, bool isWhite)
        {
            if (IsEmpty(field)) return false;
            bool targetIsWhite = field.Figur!.ToLower().Contains("_w");
            return isWhite != targetIsWhite;
        }
    }
}
