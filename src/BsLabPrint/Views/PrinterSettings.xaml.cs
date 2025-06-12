using BsLabPrint.PrinterSetting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Drawing.Text;
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
            CodeTypeCMB.Items.Add("Data Matrix");
            CodeTypeCMB.Items.Add("QR Code");
            FontWeightCMB.Items.Add("Normal");
            FontWeightCMB.Items.Add("Bold");
            FindFontAndInsertCMB();
        }

        private void FindFontAndInsertCMB()
        {
            InstalledFontCollection installedFonts = new InstalledFontCollection();
            foreach (System.Drawing.FontFamily font in installedFonts.Families)
            {
                FontTypeCMB.Items.Add(font.Name);
            }
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
        private void LoadBarcodeTypeCMB()
        {
            if (PrtSetting.Default.UseQRCode == true)
            {
                CodeTypeCMB.SelectedIndex = 1; // Yes
            }
            else
            {
                CodeTypeCMB.SelectedIndex = 0; // No
            }
        }
        private bool GetBarcodeTypeset()
        {
            if (CodeTypeCMB.SelectedIndex == 1)
            {
                return true; // Yes
            }
            else
            {
                return false; // No
            }
        }
        private void LoadFontType()
        {
            if (PrtSetting.Default.FontType == "Normal")
            {
                FontWeightCMB.SelectedIndex = 0; // Yes
            }
            else if (PrtSetting.Default.FontType == "Bold")
            {
                FontWeightCMB.SelectedIndex = 1; // No
            }
            else
            {
                FontWeightCMB.SelectedIndex = 0; // No
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
                
                PrtSetting.Default.BarcodeSize = int.Parse(BarcodeSizeBox.Text);
                PrtSetting.Default.BarcodeTextGap = int.Parse(BarcodeGapBox.Text);
                PrtSetting.Default.FontSize = int.Parse(FontSizeBox.Text);
                PrtSetting.Default.MinCharLength = int.Parse(MinCharLengthBox.Text);
                PrtSetting.Default.FontType = FontWeightCMB.SelectedItem.ToString();
                PrtSetting.Default.FontTypeFont = FontTypeCMB.SelectedItem.ToString();

                PrtSetting.Default.IsLandscape = GetlandscapeStateCmb();

                PrtSetting.Default.UseQRCode = GetBarcodeTypeset();



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

                BarcodeSizeBox.Text = PrtSetting.Default.BarcodeSize.ToString();
                FontSizeBox.Text = PrtSetting.Default.FontSize.ToString();
                BarcodeGapBox.Text = PrtSetting.Default.BarcodeTextGap.ToString();
                MinCharLengthBox.Text = PrtSetting.Default.MinCharLength.ToString();
                FontTypeCMB.Text = PrtSetting.Default.FontTypeFont;

                LoadLandscapeCMB();
                LoadBarcodeTypeCMB();
                LoadFontType();

                ApplyConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyConfig()
        {
            PrinterSettinggg.PrinterName = PrtSetting.Default.PrinterName;
            //sets default value
            PrinterSettinggg.Collate = false;
           // PrinterSettinggg.Copies = 1;
            PrinterSettinggg.Duplex = Duplex.Simplex;
           // PrinterSettinggg.FromPage = 1;
            PrinterSettinggg.DefaultPageSettings.Color = false;
           // PrinterSettinggg.DefaultPageSettings.Landscape = true;


            
            PrinterSettinggg.DefaultPageSettings.PrinterResolution = new PrinterResolution
            {
                Kind = PrinterResolutionKind.Custom,
                X = PrtSetting.Default.PrinterDpi,
                Y = PrtSetting.Default.PrinterDpi
            };
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
