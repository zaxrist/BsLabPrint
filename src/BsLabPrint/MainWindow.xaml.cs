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
            Mainvieww.BarcodeImageChanged = PrSettingView.BarcodeChangedEvent;
            PrSettingView.PrinterSettingsChanged = Mainvieww.PrinterSettings_Changed;
            PrSettingView.printpreviewClicked += Mainvieww.PrintPrevieww;
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
            PrSettingView.Visibility = Visibility.Visible;
            Mainvieww.Visibility = Visibility.Hidden;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
           // System.Windows.Forms.MessageBox.Show("Created by Zackris. GitHub: https://github.com/zaxrist");
            //AboutBox1 bb = new AboutBox1();
            //bb.ShowDialog();

            AboutWindoww mm = new AboutWindoww();
            mm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mm.Topmost = true;
            mm.Owner = this;
            mm.ShowDialog();
        }
    }
}
