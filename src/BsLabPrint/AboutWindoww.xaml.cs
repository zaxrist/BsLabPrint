using System.Diagnostics;
using System.Windows;

namespace BsLabPrint
{
    /// <summary>
    /// Interaction logic for AboutWindoww.xaml
    /// </summary>
    public partial class AboutWindoww : Window
    {
        public AboutWindoww()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
