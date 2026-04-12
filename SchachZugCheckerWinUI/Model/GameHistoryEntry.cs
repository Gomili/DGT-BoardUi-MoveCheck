using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace SchachZugCheckerWinUI.Model;

public partial class GameHistoryEntry : ObservableObject
{
    [ObservableProperty] public partial string Move { get; set; } = string.Empty;
    [ObservableProperty] public partial string CoordMove { get; set; } = string.Empty;
    [ObservableProperty] public partial string Time { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsWhite { get; set; }
    
    public Brush PlayerColorBrush => IsWhite ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
    public Brush PlayerBorderBrush => IsWhite ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.White);
}
