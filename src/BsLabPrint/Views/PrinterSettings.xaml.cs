using BsLabPrint.PrinterSetting;
using System.Drawing.Printing;
using System.Windows.Controls;

namespace BsLabPrint.Views
{
    /// <summary>
    /// Interaction logic for PrinterSettings.xaml
    /// </summary>
    public partial class PrinterSettings : UserControl
    {
        public System.Drawing.Printing.PrinterSettings _printersting { get; set; }
        public BrPrintSetup brPrint { get; set; }
        public PrinterSettings()
        {
            InitializeComponent();
            FindAllPrinter();
        }

        private void FindAllPrinter()
        {
            PrinterListCmb.Items.Clear();
            foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                PrinterListCmb.Items.Add(item);
            } 
            
        }
    }
}
