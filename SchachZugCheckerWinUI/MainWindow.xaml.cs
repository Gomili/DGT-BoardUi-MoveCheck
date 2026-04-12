using Microsoft.UI.Xaml;

namespace SchachZugCheckerWinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Schach Zug Checker WinUI";
            // In WinUI3, we set DataContext on a FrameworkElement (like the root Grid)
            if (this.Content is FrameworkElement fe)
            {
                fe.DataContext = ViewModel;
            }
        }
    }
}
