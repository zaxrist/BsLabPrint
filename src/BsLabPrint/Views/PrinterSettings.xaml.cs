using BsLabPrint.PrinterSetting;
using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BsLabPrint.Views
{
    /// <summary>
    /// Interaction logic for PrinterSettings.xaml
    /// </summary>
    public partial class PrinterSettings : UserControl, INotifyPropertyChanged
    {
        private System.Drawing.Printing.PrinterSettings _printersting;

        public System.Drawing.Printing.PrinterSettings PrinterSettinggg
        {
            get { return _printersting; }
            set { _printersting = value;}
        }

        private ImageSource _BarcodeSource;

        public ImageSource BarcodeSource
        {
            get { return _BarcodeSource; }
            set { _BarcodeSource = value; OnPropertyChanged("BarcodeSource"); }
        }

        public delegate void PrinterSettingsEvent(System.Drawing.Printing.PrinterSettings prtSetting);
        public PrinterSettingsEvent PrinterSettingsChanged;

        public PrinterSettings()
        {
            DataContext = this;
            InitializeComponent(); SetLandscapeCMB();
            PrinterSettinggg = new System.Drawing.Printing.PrinterSettings();
            FindAllPrinter();
            
        }

        private void SetLandscapeCMB()
        {
            LandscapeCmb.Items.Add("Yes");
            LandscapeCmb.Items.Add("No");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FindAllPrinter()
        {
            PrinterListCmb.Items.Clear();
            foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                PrinterListCmb.Items.Add(item);
            } 
            
        }
        public void BarcodeChangedEvent(ImageSource msg)
        {
            BarcodeSource = msg;
        }

        private void LoadLandscapeCMB()
        {
            if (PrtSetting.Default.IsLandscape == true)
            {
                LandscapeCmb.SelectedIndex = 0; // Yes
            }
            else
            {
                LandscapeCmb.SelectedIndex = 1; // No
            }
        }
        private bool GetlandscapeStateCmb()
        {
            if (LandscapeCmb.SelectedIndex == 0)
            {
                return true; // Yes
            }
            else
            {
                return false; // No
            }
        }
            private void SaveConfig()
        {
            try
            {
                PrtSetting.Default.PrinterName = PrinterListCmb.Text;
                PrtSetting.Default.PrinterDpi = int.Parse(PrinterDpiBox.Text);
                PrtSetting.Default.LabelWidth = int.Parse(LabelWidthBox.Text);
                PrtSetting.Default.LabelHeight = int.Parse(LabelHeighBox.Text);
                PrtSetting.Default.SPosY = int.Parse(StartYlbl.Text);
                PrtSetting.Default.SPosX = int.Parse(StartXlbl.Text);

                PrtSetting.Default.BarcodeWidth = int.Parse(BarcodeWidthBox.Text);
                PrtSetting.Default.BarcodeHeight = int.Parse(BarcodeHeightBox.Text);

                PrtSetting.Default.IsLandscape = GetlandscapeStateCmb();



                PrtSetting.Default.Save();

                ApplyConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LoadConfig()
        {
            try
            {
                PrinterListCmb.Text = PrtSetting.Default.PrinterName;
                PrinterDpiBox.Text = PrtSetting.Default.PrinterDpi.ToString();
                LabelWidthBox.Text = PrtSetting.Default.LabelWidth.ToString();
                LabelHeighBox.Text = PrtSetting.Default.LabelHeight.ToString();
                StartYlbl.Text = PrtSetting.Default.SPosY.ToString();
                StartXlbl.Text = PrtSetting.Default.SPosX.ToString();

                BarcodeWidthBox.Text = PrtSetting.Default.BarcodeWidth.ToString();
                BarcodeHeightBox.Text = PrtSetting.Default.BarcodeHeight.ToString();

                LoadLandscapeCMB();

                ApplyConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyConfig()
        {
            //sets default value
            PrinterSettinggg.Collate = false;
            PrinterSettinggg.Copies = 1;
            PrinterSettinggg.Duplex = Duplex.Simplex;
            PrinterSettinggg.FromPage = 1;
            PrinterSettinggg.DefaultPageSettings.Color = false;
            PrinterSettinggg.DefaultPageSettings.Landscape = true;


            PrinterSettinggg.PrinterName = PrtSetting.Default.PrinterName;
            //PrinterSettinggg.PrinterResolutions ;
            PrinterSettinggg.DefaultPageSettings.PaperSize = new PaperSize("Custom", ConvertHunInchToMM(PrtSetting.Default.LabelWidth), ConvertHunInchToMM(PrtSetting.Default.LabelHeight));
            //_printersting.DefaultPageSettings.PrinterResolution
            //_printersting.PrinterName = PrtSetting.Default.SPosY;
            //_printersting.PrinterName = PrtSetting.Default.SPosX;


            PrinterSettinggg.DefaultPageSettings.Landscape = PrtSetting.Default.IsLandscape;

            OnPrinterSettingsChanged();
        }

        private int ConvertHunInchToMM(int hundredthInch)
        {
            try
            {
                double dd = hundredthInch;
                dd = (dd / 25.4) * 100;
                return (int)dd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }
        public void OnPrinterSettingsChanged()
        {
            if (PrinterSettingsChanged != null)
            {
                PrinterSettingsChanged.Invoke(PrinterSettinggg);
            }
        }

        private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            MessageBox.Show("Printer Configuration Saved");
        }

        public delegate void PrintPreviewEventHandler();
        public event PrintPreviewEventHandler printpreviewClicked;
        private void PrintPreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            printpreviewClicked.Invoke();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
    }
}
