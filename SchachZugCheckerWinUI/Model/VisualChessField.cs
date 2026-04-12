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
        [ObservableProperty] public partial int Row { get; set; }
        [ObservableProperty] public partial int Column { get; set; }
        [ObservableProperty] public partial bool IsDark { get; set; }
        [ObservableProperty] public partial double Opacity { get; set; } = 1.0;

        public Microsoft.UI.Xaml.Media.Brush BackgroundBrush => IsDark ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.SaddleBrown) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Wheat);
        
        public Microsoft.UI.Xaml.Visibility HighlightVisibility => FieldState == FieldState.None ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
        
        public Microsoft.UI.Xaml.Media.Brush HighlightBrush => FieldState == FieldState.BlueState ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);

        // Notify UI about dependent property changes
        partial void OnIsDarkChanged(bool value) => OnPropertyChanged(nameof(BackgroundBrush));
        partial void OnFieldStateChanged(FieldState value)
        {
            OnPropertyChanged(nameof(HighlightVisibility));
            OnPropertyChanged(nameof(HighlightBrush));
        }
    }
}
