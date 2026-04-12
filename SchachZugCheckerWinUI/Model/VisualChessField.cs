using CommunityToolkit.Mvvm.ComponentModel;

namespace SchachZugCheckerWinUI.Model
{
    public enum FieldState
    {
        None = 0, // Field is empty and has no color
        BlueState = 1, // Figure can set on this field
        Red = 2, // Figure can hit the figure on the field
    }

    public partial class VisualChessField : ObservableObject
    {
        [ObservableProperty] public partial string? Figur { get; set; }
        [ObservableProperty] public partial FieldState FieldState { get; set; } = FieldState.None;
        [ObservableProperty] public partial int ArrayPos { get; set; }
        [ObservableProperty] public partial string BoardPos { get; set; } = string.Empty;
    }
}
