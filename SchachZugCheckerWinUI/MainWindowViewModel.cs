using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
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
    [ObservableProperty] public partial bool IsWhiteTurn { get; set; } = true;
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
        if (MoveHelp)
        {
            HighlightLegalMoves(field);
        }
    }

    public void OnDrop(VisualChessField toField)
    {
        if (_dgtDriver.ConState || _selectedField == null) return;
        if (toField == _selectedField)
        {
            ClearHighlights();
            _selectedField = null;
            return;
        }

        var legalMoves = ChessEngine.GetLegalMoves(_selectedField, ChessFields);
        if (legalMoves.Contains(toField.BoardPos))
        {
            ExecuteMove(_selectedField, toField);
        }
        else
        {
            ClearHighlights();
            _selectedField = null;
        }
    }

    public void OnDragCompleted()
    {
        // We only clear highlights if we didn't just execute a move (which clears highlights anyway)
        // But to be safe:
        ClearHighlights();
        _selectedField = null;
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

    private void HighlightLegalMoves(VisualChessField selectedField)
    {
        ClearHighlights();
        var legalMoves = ChessEngine.GetLegalMoves(selectedField, ChessFields);
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
        string moveText = $"{from.BoardPos}-{to.BoardPos}";
        GameHistory.Insert(0, moveText); // Newest at top

        to.Figur = from.Figur;
        from.Figur = Files.Leer;

        IsWhiteTurn = !IsWhiteTurn;
        OnPropertyChanged(nameof(WhiteTurnBrush));
        OnPropertyChanged(nameof(BlackTurnBrush));
        OnPropertyChanged(nameof(WhiteTurnBorderBrush));
        OnPropertyChanged(nameof(BlackTurnBorderBrush));

        ClearHighlights();
        _selectedField = null;
    }

    partial void OnMoveHelpChanged(bool value)
    {
        if (_dgtDriver is not null)
            _dgtDriver.MoveHelp = value;
    }
    
    private DgtSerDriver _dgtDriver { get; set; }

    public MainWindowViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
            if (MoveHelp) HighlightLegalMoves(liftedField);
        }
        
        if (placedField != null && _lastLiftedField != null)
        {
            string moveText = $"{_lastLiftedField.BoardPos}-{placedField.BoardPos}";
            if (!GameHistory.Contains(moveText) || GameHistory.FirstOrDefault() != moveText)
            {
                GameHistory.Insert(0, moveText);
                IsWhiteTurn = !IsWhiteTurn;
                OnPropertyChanged(nameof(WhiteTurnBrush));
                OnPropertyChanged(nameof(BlackTurnBrush));
                OnPropertyChanged(nameof(WhiteTurnBorderBrush));
                OnPropertyChanged(nameof(BlackTurnBorderBrush));
            }
            
            ClearHighlights();
            _lastLiftedField = null;
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
