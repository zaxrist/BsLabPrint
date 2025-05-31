using BsLabPrint.Views;
using System.Windows;

namespace BsLabPrint
{
    public partial class MainWindow : Window
    {
        MainView Mainvieww = new MainView();
        PrinterSettings PrSettingView = new PrinterSettings();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainviewWindow.Children.Add(Mainvieww);
            MainviewWindow.Children.Add(PrSettingView);
            PrSettingView.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrSettingView.Visibility = Visibility.Hidden;
            Mainvieww.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            PrSettingView.PrinterView.Source = Mainvieww.BarcodeImage;
            PrSettingView.Visibility = Visibility.Visible;
            Mainvieww.Visibility = Visibility.Hidden;

        }
    }
}
