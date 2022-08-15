using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test;

namespace SchachZugCheckerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void Bt_Open_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DgtSerDriver = new DgtSerDriver();
            _viewModel.DgtSerDriver.Open();
            _viewModel.OpenVisi = Visibility.Hidden;
            _viewModel.CloseVisi = Visibility.Visible;
            _viewModel.DgtSerDriver.UpdateBoard += DgtSerDriverOnUpdateBoard; 
        }

        private void DgtSerDriverOnUpdateBoard(byte[] data)
        {
            _viewModel.UpdateBoard(data);
        }

        private void Bt_Close_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DgtSerDriver != null)
            {
                _viewModel.DgtSerDriver.Close();
                _viewModel.OpenVisi = Visibility.Visible;
                _viewModel.CloseVisi = Visibility.Hidden;

                _viewModel.DgtSerDriver.UpdateBoard -= DgtSerDriverOnUpdateBoard;
            }
        }
    }
}
