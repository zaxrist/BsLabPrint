
using System.Drawing.Printing;
namespace BsLabPrint.PrinterSetting
{
    public class BrPrintSetup
    {
        public PrinterSettings _printersetting { get; private set; }
        public PageSettings _pageSetting { get; private set; }
        public bool printerHadbeenSetup { get; private set; } = false;
        public BrPrintSetup(PrinterSettings printersetting, 
                            PageSettings pageSetting)
        {
            _printersetting = printersetting;
            _pageSetting = pageSetting;
            InitlizeDefaultSettings();
        }

        // "SATO CL4NX Plus 305dpi"
        public void setupPrinter()
        {
            PrinterResolution pp = new PrinterResolution();
            //pp.x
            //_pageSetting.PrinterResolution =
        }

        private void InitlizeDefaultSettings()
        {
            //sets default value
            _printersetting.Collate = false;
            _printersetting.Copies = 1;
            _printersetting.Duplex = Duplex.Simplex;
            _printersetting.FromPage = 1;

            //sets the default value
            _pageSetting.Color = false;
            _pageSetting.Landscape = true;


        }
    }
}
