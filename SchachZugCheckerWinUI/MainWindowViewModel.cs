using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchachZugCheckerWinUI.Model;
using SchachZugCheckerWinUI.Logic;
using Test;

namespace SchachZugCheckerWinUI;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty] public partial ObservableCollection<VisualChessField> ChessFields { get; set; } = new();
    [ObservableProperty] public partial ObservableCollection<string> GameHistory { get; set; } = new();

    [ObservableProperty] public partial Visibility CloseVisi { get; set; } = Visibility.Collapsed;
    [ObservableProperty] public partial Visibility OpenVisi { get; set; } = Visibility.Visible;
    [ObservableProperty] public partial Brush StatusColorBrush { get; set; } = new SolidColorBrush(Colors.DarkRed);
    [ObservableProperty] public partial bool TextBoxEnable { get; set; } = true;
    [ObservableProperty] public partial string Comport { get; set; } = "COM3";
    [ObservableProperty] public partial bool MoveHelp { get; set; } = false;
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(WhiteTurnBrush))]
    [NotifyPropertyChangedFor(nameof(BlackTurnBrush))]
    [NotifyPropertyChangedFor(nameof(WhiteTurnBorderBrush))]
    [NotifyPropertyChangedFor(nameof(BlackTurnBorderBrush))]
    public partial bool IsWhiteTurn { get; set; } = true;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WhiteTimeStr))] public partial TimeSpan WhiteTime { get; set; } = TimeSpan.Zero;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(BlackTimeStr))] public partial TimeSpan BlackTime { get; set; } = TimeSpan.Zero;

    public string WhiteTimeStr => WhiteTime.ToString(@"mm\:ss");
    public string BlackTimeStr => BlackTime.ToString(@"mm\:ss");

    private DispatcherTimer? _gameTimer;
    private bool _timerStarted = false;

    public Brush WhiteTurnBrush => IsWhiteTurn ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Transparent);
    public Brush BlackTurnBrush => !IsWhiteTurn ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Transparent);
    public Brush WhiteTurnBorderBrush => IsWhiteTurn ? new SolidColorBrush(Colors.Gold) : new SolidColorBrush(Colors.Gray);
    public Brush BlackTurnBorderBrush => !IsWhiteTurn ? new SolidColorBrush(Colors.Gold) : new SolidColorBrush(Colors.Gray);

    private VisualChessField? _selectedField;
    private VisualChessField? _lastLiftedField;
    private string? _lastLiftedFigur;
    private string? _draggedFigur;

    private ChessState _chessState = new();

    public Func<Task<bool>>? AskConfirmationAsync { get; set; }

    [RelayCommand]
    public void FieldTapped(VisualChessField field)
    {
        FieldClicked(field);
    }

    public void OnDragStarting(VisualChessField field)
    {
        if (_dgtDriver.ConState) return;
        if (string.IsNullOrEmpty(field.Figur) || field.Figur == Files.Leer) return;

        // Only allow moving own pieces
        bool isWhitePiece = field.Figur.ToLower().Contains("_w");
        if (isWhitePiece != IsWhiteTurn) return;

        // Start timer if first move by white
        if (!_timerStarted && isWhitePiece)
        {
            _timerStarted = true;
            _gameTimer?.Start();
        }

        _selectedField = field;
        _draggedFigur = field.Figur;

        // Delay hiding the figure and showing highlights so WinUI can capture the drag visual correctly
        Task.Run(async () =>
        {
            await Task.Delay(250); // Increased delay slightly
            _dispatcherQueue.TryEnqueue(() => 
            {
                if (_selectedField == field)
                {
                    field.Opacity = 0.0;
                    if (MoveHelp)
                    {
                        HighlightLegalMoves(field);
                    }
                }
            });
        });
    }

    public void OnDrop(VisualChessField toField)
    {
        if (_dgtDriver.ConState || _selectedField == null) return;
        
        ClearHighlights();

        if (toField == _selectedField) return;

        var legalMoves = ChessEngine.GetLegalMoves(_selectedField, ChessFields, _draggedFigur, _chessState);
        if (legalMoves.Contains(toField.BoardPos))
        {
            ExecuteMove(_selectedField, toField);
        }
    }

    public void OnDragCompleted()
    {
        if (_selectedField != null)
        {
            if (!string.IsNullOrEmpty(_draggedFigur))
            {
                _selectedField.Figur = _draggedFigur;
            }
            _selectedField.Opacity = 1.0;
        }
        ClearHighlights();
        _selectedField = null;
        _draggedFigur = null;
    }

    [RelayCommand]
    private void FieldClicked(VisualChessField field)
    {
        if (_dgtDriver.ConState) return;

        if (_selectedField == null)
        {
            if (string.IsNullOrEmpty(field.Figur) || field.Figur == Files.Leer) return;

            // Only allow selecting own pieces
            bool isWhitePiece = field.Figur.ToLower().Contains("_w");
            if (isWhitePiece != IsWhiteTurn) return;

            _selectedField = field;
            if (MoveHelp)
            {
                HighlightLegalMoves(field);
            }
        }
        else
        {
            if (field == _selectedField)
            {
                ClearHighlights();
                _selectedField = null;
                return;
            }

            if (field.FieldState != FieldState.None) // Valid move detected via highlights
            {
                ExecuteMove(_selectedField, field);
            }
            else
            {
                // Select another figure or clear
                ClearHighlights();
                if (!string.IsNullOrEmpty(field.Figur) && field.Figur != Files.Leer)
                {
                    _selectedField = field;
                    if (MoveHelp) HighlightLegalMoves(field);
                }
                else
                {
                    _selectedField = null;
                }
            }
        }
    }

    private void HighlightLegalMoves(VisualChessField selectedField, string? overrideFigur = null)
    {
        ClearHighlights();
        var legalMoves = ChessEngine.GetLegalMoves(selectedField, ChessFields, overrideFigur, _chessState);
        foreach (var pos in legalMoves)
        {
            var field = ChessFields.FirstOrDefault(f => f.BoardPos == pos);
            if (field != null)
            {
                field.FieldState = string.IsNullOrEmpty(field.Figur) || field.Figur == Files.Leer 
                                    ? FieldState.BlueState 
                                    : FieldState.Red;
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var f in ChessFields) f.FieldState = FieldState.None;
    }

    private void ExecuteMove(VisualChessField from, VisualChessField to)
    {
        string? figurToMove = !string.IsNullOrEmpty(_draggedFigur) ? _draggedFigur : from.Figur;
        bool isWhite = figurToMove?.ToLower().Contains("_w") ?? false;
        string moveText = $"{from.BoardPos}-{to.BoardPos}";

        // --- SPECIAL MOVES LOGIC ---
        
        // 1. Castling detection and execution
        if (figurToMove?.ToLower().Contains("_wk") == true || figurToMove?.ToLower().Contains("_sk") == true)
        {
            int colDiff = to.Column - from.Column;
            if (Math.Abs(colDiff) == 2)
            {
                // Castling!
                moveText = colDiff > 0 ? "O-O" : "O-O-O";
                int rookSourceCol = colDiff > 0 ? 7 : 0; // H or A
                int rookDestCol = colDiff > 0 ? to.Column - 1 : to.Column + 1;
                var rookFrom = ChessFields.FirstOrDefault(f => f.Row == from.Row && f.Column == rookSourceCol);
                var rookTo = ChessFields.FirstOrDefault(f => f.Row == from.Row && f.Column == rookDestCol);
                if (rookFrom != null && rookTo != null)
                {
                    rookTo.Figur = rookFrom.Figur;
                    rookFrom.Figur = Files.Leer;
                }
            }
        }

        // 2. En Passant detection and execution
        if ((figurToMove?.ToLower().Contains("_wb") == true || figurToMove?.ToLower().Contains("_sb") == true) && to.BoardPos == _chessState.EnPassantTarget)
        {
            // Capture the pawn behind the target square
            int captureRow = from.Row; // The row where the enemy pawn was
            int captureCol = to.Column;
            var capturedPawnField = ChessFields.FirstOrDefault(f => f.Row == captureRow && f.Column == captureCol);
            if (capturedPawnField != null) capturedPawnField.Figur = Files.Leer;
        }

        // --- UPDATE CHESS STATE ---

        // Update moved flags
        if (from.BoardPos == "E1") _chessState.WhiteKingMoved = true;
        if (from.BoardPos == "E8") _chessState.BlackKingMoved = true;
        if (from.BoardPos == "A1") _chessState.WhiteRookAMoved = true;
        if (from.BoardPos == "H1") _chessState.WhiteRookHMoved = true;
        if (from.BoardPos == "A8") _chessState.BlackRookAMoved = true;
        if (from.BoardPos == "H8") _chessState.BlackRookHMoved = true;

        // Update En Passant target
        _chessState.EnPassantTarget = null;
        if (figurToMove?.ToLower().Contains("_wb") == true || figurToMove?.ToLower().Contains("_sb") == true)
        {
            if (Math.Abs(to.Row - from.Row) == 2)
            {
                // Double step!
                int targetRow = (from.Row + to.Row) / 2;
                var targetField = ChessFields.FirstOrDefault(f => f.Row == targetRow && f.Column == from.Column);
                if (targetField != null) _chessState.EnPassantTarget = targetField.BoardPos;
            }
        }

        // --- EXECUTE BASE MOVE ---
        GameHistory.Insert(0, moveText); // Newest at top
        to.Figur = figurToMove;
        from.Figur = Files.Leer;
        from.Opacity = 1.0; // Reset opacity for the source field

        IsWhiteTurn = !IsWhiteTurn;

        ClearHighlights();
        _selectedField = null;
        _draggedFigur = null;
        CheckForCheck();
    }

    partial void OnMoveHelpChanged(bool value)
    {
        if (_dgtDriver is not null)
            _dgtDriver.MoveHelp = value;

        if (value)
        {
            if (_lastLiftedField != null) HighlightLegalMoves(_lastLiftedField, _lastLiftedFigur);
            else if (_selectedField != null) HighlightLegalMoves(_selectedField, _draggedFigur);
        }
        else
        {
            ClearHighlights();
        }
    }
    
    private DgtSerDriver _dgtDriver { get; set; }

    public MainWindowViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        ElementSoundPlayer.State = ElementSoundPlayerState.On;
        ElementSoundPlayer.Volume = 1.0;
        InitializeBoard();
        ResetBoard();
        
        _gameTimer = new DispatcherTimer();
        _gameTimer.Interval = TimeSpan.FromSeconds(1);
        _gameTimer.Tick += (s, e) =>
        {
            if (IsWhiteTurn) WhiteTime = WhiteTime.Add(TimeSpan.FromSeconds(1));
            else BlackTime = BlackTime.Add(TimeSpan.FromSeconds(1));
        };

        _dgtDriver = new DgtSerDriver();
        _dgtDriver.UpdateBoard += DgtSerDriverOnUpdateBoard;
    }

    private void InitializeBoard()
    {
        ChessFields.Clear();
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                bool isDark = (r + c) % 2 != 0;
                var field = new VisualChessField
                {
                    Row = r,
                    Column = c,
                    IsDark = isDark,
                    BoardPos = $"{(char)('A' + c)}{8 - r}",
                    Figur = Files.Leer
                };
                ChessFields.Add(field);
            }
        }
    }

    private void ResetBoard()
    {
        _timerStarted = false;
        _gameTimer?.Stop();
        WhiteTime = TimeSpan.Zero;
        BlackTime = TimeSpan.Zero;
        _chessState = new ChessState();
        IsWhiteTurn = true;
        GameHistory.Clear();

        foreach (var field in ChessFields) field.Figur = Files.Leer;

        SetFigur("A1", Files.WT); SetFigur("B1", Files.WS); SetFigur("C1", Files.WL); SetFigur("D1", Files.WD);
        SetFigur("E1", Files.WK); SetFigur("F1", Files.WL); SetFigur("G1", Files.WS); SetFigur("H1", Files.WT);
        SetFigur("A2", Files.WB); SetFigur("B2", Files.WB); SetFigur("C2", Files.WB); SetFigur("D2", Files.WB);
        SetFigur("E2", Files.WB); SetFigur("F2", Files.WB); SetFigur("G2", Files.WB); SetFigur("H2", Files.WB);

        SetFigur("A8", Files.ST); SetFigur("B8", Files.SS); SetFigur("C8", Files.SL); SetFigur("D8", Files.SD);
        SetFigur("E8", Files.SK); SetFigur("F8", Files.SL); SetFigur("G8", Files.SS); SetFigur("H8", Files.ST);
        SetFigur("A7", Files.SB); SetFigur("B7", Files.SB); SetFigur("C7", Files.SB); SetFigur("D7", Files.SB);
        SetFigur("E7", Files.SB); SetFigur("F7", Files.SB); SetFigur("G7", Files.SB); SetFigur("H7", Files.SB);
    }

    private void SetFigur(string pos, string figur)
    {
        var field = ChessFields.FirstOrDefault(f => f.BoardPos == pos);
        if (field != null) field.Figur = figur;
    }

    [RelayCommand]
    private async Task Open()
    {
        if (!IsOriginalState() && AskConfirmationAsync != null)
        {
            if (!await AskConfirmationAsync()) return;
        }

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
                    ResetBoard(); // Reset UI and history when connecting
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

        VisualChessField? liftedField = null;
        VisualChessField? placedField = null;
        string? placedFigur = null;

        for (int i = 0; i < 64; i++)
        {
            var field = ChessFields[i];
            var newFigur = (Figuren)data[i + 3];
            string? newFigurPath = GetFigur(newFigur);

            if (field.Figur != newFigurPath)
            {
                // Figured lifted?
                if (IsNotEmpty(field.Figur) && IsEmpty(newFigurPath))
                {
                    liftedField = field;
                    _lastLiftedFigur = field.Figur;
                    
                    // Start timer if first move by white
                    if (!_timerStarted && (field.Figur?.ToLower().Contains("_w") ?? false) && IsWhiteTurn)
                    {
                        _timerStarted = true;
                        _gameTimer?.Start();
                    }
                }
                // Figure placed?
                else if (IsEmpty(field.Figur) && IsNotEmpty(newFigurPath))
                {
                    placedField = field;
                    placedFigur = newFigurPath;
                }
                
                field.Figur = newFigurPath;
            }
        }

        if (liftedField != null)
        {
            _lastLiftedField = liftedField;
            if (MoveHelp) HighlightLegalMoves(liftedField, _lastLiftedFigur);
        }
        
        if (placedField != null && _lastLiftedField != null)
        {
            string moveText = $"{_lastLiftedField.BoardPos}-{placedField.BoardPos}";
            bool isWhite = placedFigur?.ToLower().Contains("_w") ?? false;

            // Detect Castling in DGT
            if (placedFigur?.ToLower().Contains("k") == true && Math.Abs(placedField.Column - _lastLiftedField.Column) == 2)
            {
                moveText = placedField.Column > _lastLiftedField.Column ? "O-O" : "O-O-O";
            }
            
            // Ignore Rook moves that are part of a castling (if King already moved)
            bool isRookCompletingCastling = false;
            if (placedFigur?.ToLower().Contains("t") == true)
            {
                var lastMove = GameHistory.FirstOrDefault();
                if (lastMove == "O-O" || lastMove == "O-O-O")
                {
                    // Check if this rook move matches the castling side
                    // If O-O, White Rook H1-F1 or Black Rook H8-F8
                    // If O-O-O, White Rook A1-D1 or Black Rook A8-D8
                    if (isWhite)
                    {
                        if (lastMove == "O-O" && _lastLiftedField.BoardPos == "H1" && placedField.BoardPos == "F1") isRookCompletingCastling = true;
                        if (lastMove == "O-O-O" && _lastLiftedField.BoardPos == "A1" && placedField.BoardPos == "D1") isRookCompletingCastling = true;
                    }
                    else
                    {
                        if (lastMove == "O-O" && _lastLiftedField.BoardPos == "H8" && placedField.BoardPos == "F8") isRookCompletingCastling = true;
                        if (lastMove == "O-O-O" && _lastLiftedField.BoardPos == "A8" && placedField.BoardPos == "D8") isRookCompletingCastling = true;
                    }
                }
            }

            if (!isRookCompletingCastling && (!GameHistory.Contains(moveText) || GameHistory.FirstOrDefault() != moveText))
            {
                GameHistory.Insert(0, moveText);
                
                // Update ChessState from DGT move
                if (_lastLiftedField.BoardPos == "E1") _chessState.WhiteKingMoved = true;
                if (_lastLiftedField.BoardPos == "E8") _chessState.BlackKingMoved = true;
                if (_lastLiftedField.BoardPos == "A1") _chessState.WhiteRookAMoved = true;
                if (_lastLiftedField.BoardPos == "H1") _chessState.WhiteRookHMoved = true;
                if (_lastLiftedField.BoardPos == "A8") _chessState.BlackRookAMoved = true;
                if (_lastLiftedField.BoardPos == "H8") _chessState.BlackRookHMoved = true;

                // En Passant target update for DGT
                _chessState.EnPassantTarget = null;
                if (placedFigur?.ToLower().Contains("b") == true && Math.Abs(placedField.Row - _lastLiftedField.Row) == 2)
                {
                    int targetRow = (_lastLiftedField.Row + placedField.Row) / 2;
                    var targetField = ChessFields.FirstOrDefault(f => f.Row == targetRow && f.Column == _lastLiftedField.Column);
                    if (targetField != null) _chessState.EnPassantTarget = targetField.BoardPos;
                }

                IsWhiteTurn = !IsWhiteTurn;
            }
            
            ClearHighlights();
            _lastLiftedField = null;
            _lastLiftedFigur = null;
        }
    }

    private void CheckForCheck()
    {
        if (ChessEngine.IsInCheck(IsWhiteTurn, ChessFields))
        {
            try
            {
                // Play sound twice with a tiny delay to simulate "double volume" / more impact
                ElementSoundPlayer.Play(ElementSoundKind.Invoke);
                Task.Delay(50).ContinueWith(_ => _dispatcherQueue.TryEnqueue(() => ElementSoundPlayer.Play(ElementSoundKind.Invoke)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewModel] Failed to play sound: {ex.Message}");
            }
        }
    }

    private bool IsOriginalState()
    {
        foreach (var field in ChessFields)
        {
            string expected = GetInitialFigurAt(field.BoardPos);
            if (field.Figur != expected) return false;
        }
        return true;
    }

    private string GetInitialFigurAt(string pos)
    {
        if (pos.Length < 2) return Files.Leer;
        char col = pos[0];
        char row = pos[1];

        if (row == '2') return Files.WB;
        if (row == '7') return Files.SB;
        if (row == '1')
        {
            return col switch
            {
                'A' or 'H' => Files.WT,
                'B' or 'G' => Files.WS,
                'C' or 'F' => Files.WL,
                'D' => Files.WD,
                'E' => Files.WK,
                _ => Files.Leer
            };
        }
        if (row == '8')
        {
            return col switch
            {
                'A' or 'H' => Files.ST,
                'B' or 'G' => Files.SS,
                'C' or 'F' => Files.SL,
                'D' => Files.SD,
                'E' => Files.SK,
                _ => Files.Leer
            };
        }
        return Files.Leer;
    }

    private bool IsEmpty(string? figur) => string.IsNullOrEmpty(figur) || figur == Files.Leer;
    private bool IsNotEmpty(string? figur) => !IsEmpty(figur);

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
