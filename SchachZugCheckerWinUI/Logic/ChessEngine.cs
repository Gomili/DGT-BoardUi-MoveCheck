using System;
using System.Collections.Generic;
using System.Linq;
using SchachZugCheckerWinUI.Model;

namespace SchachZugCheckerWinUI.Logic
{
    public class ChessState
    {
        public bool WhiteKingMoved { get; set; }
        public bool BlackKingMoved { get; set; }
        public bool WhiteRookAMoved { get; set; } // A1
        public bool WhiteRookHMoved { get; set; } // H1
        public bool BlackRookAMoved { get; set; } // A8
        public bool BlackRookHMoved { get; set; } // H8
        public string? EnPassantTarget { get; set; } // Position where a pawn can be captured (e.g. "E3")
    }

    public class ChessEngine
    {
        public static List<string> GetLegalMoves(VisualChessField selectedField, IList<VisualChessField> allFields, string? overrideFigur = null, ChessState? state = null)
        {
            List<string> legalMoves = new List<string>();
            string? currentFigur = overrideFigur ?? selectedField.Figur;

            if (string.IsNullOrEmpty(currentFigur) || currentFigur.Contains("Leer"))
                return legalMoves;

            string figurPath = currentFigur.ToLower();
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
                    if (fc != null)
                    {
                        if (IsOpponent(fc, isWhite))
                        {
                            legalMoves.Add(fc.BoardPos);
                        }
                        else if (state != null && fc.BoardPos == state.EnPassantTarget)
                        {
                            // En Passant
                            legalMoves.Add(fc.BoardPos);
                        }
                    }
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

                // Castling
                if (state != null)
                {
                    bool kingMoved = isWhite ? state.WhiteKingMoved : state.BlackKingMoved;
                    if (!kingMoved)
                    {
                        // Kingside
                        bool rookHMoved = isWhite ? state.WhiteRookHMoved : state.BlackRookHMoved;
                        if (!rookHMoved)
                        {
                            var f1 = GetField(startRow, startCol + 1);
                            var f2 = GetField(startRow, startCol + 2);
                            if (f1 != null && IsEmpty(f1) && f2 != null && IsEmpty(f2))
                            {
                                // Path must not be attacked
                                if (!IsSquareAttacked(selectedField.BoardPos, !isWhite, allFields) &&
                                    !IsSquareAttacked(f1.BoardPos, !isWhite, allFields) &&
                                    !IsSquareAttacked(f2.BoardPos, !isWhite, allFields))
                                {
                                    legalMoves.Add(f2.BoardPos);
                                }
                            }
                        }
                        // Queenside
                        bool rookAMoved = isWhite ? state.WhiteRookAMoved : state.BlackRookAMoved;
                        if (!rookAMoved)
                        {
                            var f1 = GetField(startRow, startCol - 1);
                            var f2 = GetField(startRow, startCol - 2);
                            var f3 = GetField(startRow, startCol - 3);
                            if (f1 != null && IsEmpty(f1) && f2 != null && IsEmpty(f2) && f3 != null && IsEmpty(f3))
                            {
                                // Path (king's squares) must not be attacked
                                if (!IsSquareAttacked(selectedField.BoardPos, !isWhite, allFields) &&
                                    !IsSquareAttacked(f1.BoardPos, !isWhite, allFields) &&
                                    !IsSquareAttacked(f2.BoardPos, !isWhite, allFields))
                                {
                                    legalMoves.Add(f2.BoardPos);
                                }
                            }
                        }
                    }
                }
            }

            // Optional: Filter moves that leave king in check. 
            var filteredMoves = new List<string>();
            foreach (var move in legalMoves)
            {
                if (!WouldBeInCheck(selectedField, move, allFields, isWhite))
                {
                    filteredMoves.Add(move);
                }
            }

            return filteredMoves;
        }

        private static bool WouldBeInCheck(VisualChessField from, string toPos, IList<VisualChessField> allFields, bool isWhite)
        {
            var toField = allFields.FirstOrDefault(f => f.BoardPos == toPos);
            if (toField == null) return false;

            string? oldFromFigur = from.Figur;
            string? oldToFigur = toField.Figur;

            from.Figur = Files.Leer;
            toField.Figur = oldFromFigur;

            string kingTag = isWhite ? "_wk" : "_sk";
            var kingField = allFields.FirstOrDefault(f => f.Figur != null && f.Figur.ToLower().Contains(kingTag));
            
            bool inCheck = false;
            if (kingField != null)
            {
                inCheck = IsSquareAttacked(kingField.BoardPos, !isWhite, allFields);
            }

            from.Figur = oldFromFigur;
            toField.Figur = oldToFigur;

            return inCheck;
        }

        public static bool IsInCheck(bool isWhite, IList<VisualChessField> allFields)
        {
            var kingField = allFields.FirstOrDefault(f => f.Figur != null && f.Figur.ToLower().Contains(isWhite ? "_wk" : "_sk"));
            if (kingField == null) return false;
            return IsSquareAttacked(kingField.BoardPos, !isWhite, allFields);
        }

        public static bool IsSquareAttacked(string boardPos, bool attackerIsWhite, IList<VisualChessField> allFields)
        {
            var targetField = allFields.FirstOrDefault(f => f.BoardPos == boardPos);
            if (targetField == null) return false;

            int row = targetField.Row;
            int col = targetField.Column;

            int[][] rookDirs = { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 } };
            int[][] bishopDirs = { new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 } };
            int[][] knightMoves = { 
                new[] { -2, -1 }, new[] { -2, 1 }, new[] { -1, -2 }, new[] { -1, 2 },
                new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 } 
            };

            VisualChessField? GetField(int r, int c) => allFields.FirstOrDefault(f => f.Row == r && f.Column == c);

            // 1. Knights
            foreach (var m in knightMoves)
            {
                var f = GetField(row + m[0], col + m[1]);
                if (f != null && f.Figur != null && f.Figur.ToLower().Contains(attackerIsWhite ? "_ws" : "_ss")) return true;
            }

            // 2. Pawns
            int pawnDir = attackerIsWhite ? 1 : -1; 
            int[] pawnCols = { col - 1, col + 1 };
            foreach (var pc in pawnCols)
            {
                var f = GetField(row + pawnDir, pc);
                if (f != null && f.Figur != null && f.Figur.ToLower().Contains(attackerIsWhite ? "_wb" : "_sb")) return true;
            }

            // 3. Sliding (Rook/Queen)
            foreach (var d in rookDirs)
            {
                for (int i = 1; i < 8; i++)
                {
                    var f = GetField(row + d[0] * i, col + d[1] * i);
                    if (f == null) break;
                    if (!IsEmpty(f))
                    {
                        string fig = f.Figur!.ToLower();
                        if (fig.Contains(attackerIsWhite ? "_w" : "_s"))
                        {
                            if (fig.Contains("t") || fig.Contains("d")) return true;
                        }
                        break;
                    }
                }
            }

            // 4. Sliding (Bishop/Queen)
            foreach (var d in bishopDirs)
            {
                for (int i = 1; i < 8; i++)
                {
                    var f = GetField(row + d[0] * i, col + d[1] * i);
                    if (f == null) break;
                    if (!IsEmpty(f))
                    {
                        string fig = f.Figur!.ToLower();
                        if (fig.Contains(attackerIsWhite ? "_w" : "_s"))
                        {
                            if (fig.Contains("l") || fig.Contains("d")) return true;
                        }
                        break;
                    }
                }
            }

            // 5. King
            foreach (var d in rookDirs.Concat(bishopDirs))
            {
                var f = GetField(row + d[0], col + d[1]);
                if (f != null && f.Figur != null && f.Figur.ToLower().Contains(attackerIsWhite ? "_wk" : "_sk")) return true;
            }

            return false;
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
