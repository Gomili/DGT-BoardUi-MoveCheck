using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using System;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
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

            // Set window size to 1920x1080 and restrict resizing
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(1600, 1000));

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = true;
                presenter.IsMaximizable = true;
            }

            ViewModel.AskConfirmationAsync = async (title, message) =>
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
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
            
            // Hide the "Move to" text and the arrow glyph during drag
            args.DragUIOverride.IsCaptionVisible = false;
            args.DragUIOverride.IsGlyphVisible = false;
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
