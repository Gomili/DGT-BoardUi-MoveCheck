using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using System;
using System.Threading.Tasks;
using SchachZugCheckerWinUI.Model;

namespace SchachZugCheckerWinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Schach Zug Checker WinUI";

            ViewModel.AskConfirmationAsync = async () =>
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Stellung zurücksetzen?",
                    Content = "Der Verbindungsaufbau zum DGT Board wird die aktuelle manuelle Stellung durch die Board-Stellung ersetzen. Möchten Sie fortfahren?",
                    PrimaryButtonText = "Ja",
                    CloseButtonText = "Abbrechen",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();
                return result == ContentDialogResult.Primary;
            };

            // In WinUI3, we set DataContext on a FrameworkElement (like the root Grid)
            if (this.Content is FrameworkElement fe)
            {
                fe.DataContext = ViewModel;
            }
        }

        private void Image_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            if (sender is FrameworkElement fe && fe.DataContext is VisualChessField field)
            {
                ViewModel.OnDragStarting(field);
                args.Data.SetText(field.BoardPos);
            }
        }

        private void Image_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            ViewModel.OnDragCompleted();
        }

        private void Field_DragOver(object sender, DragEventArgs args)
        {
            args.AcceptedOperation = DataPackageOperation.Move;
        }

        private void Field_Drop(object sender, DragEventArgs args)
        {
            if (sender is FrameworkElement fe && fe.DataContext is VisualChessField field)
            {
                ViewModel.OnDrop(field);
            }
        }

        private void Field_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is VisualChessField field)
            {
                ViewModel.FieldTapped(field);
            }
        }
    }
}
