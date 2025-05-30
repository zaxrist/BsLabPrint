using BsLabPrint.Views;
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

namespace BsLabPrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
