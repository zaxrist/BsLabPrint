using BsLabPrint.Modules;
using BsLabPrint.PrinterSetting;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

namespace BsLabPrint.Views
{
    public partial class MainView : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        PrintDocument Printdocument { get; set; }

        public delegate void BarcodeStatus(ImageSource img);
        public BarcodeStatus BarcodeImageChanged;
        public MainView()
        {
            DataContext = this;
            InitializeComponent();
            Printdocument = new PrintDocument();
            Printdocument.PrintPage += Printdocument_PrintPage;
            LoadPrintQty();
        }

        private string _PrinterNameText;

        public string PrinterNameText
        {
            get { return _PrinterNameText; }
            set { _PrinterNameText = value; OnPropertyChanged(); }
        }

        private void Printdocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {

                // Get the printable area
                Rectangle printableArea = e.PageBounds; // Use the entire page or MarginBounds for margins

                // Calculate the scaled dimensions
                double imageRatio = (double)BarcodeImage.Width / BarcodeImage.Height;
                int scaledWidth = printableArea.Width;
                int scaledHeight = (int)(printableArea.Width / imageRatio);

                // If the scaled height exceeds the printable area's height, adjust width
                if (scaledHeight > printableArea.Height)
                {
                    scaledHeight = printableArea.Height;
                    scaledWidth = (int)(printableArea.Height * imageRatio);
                }

                Bitmap bb = Barcode.ImageSourceToBitmap(BarcodeImage);
                
                // Draw the scaled image
                e.Graphics.DrawImage(bb,
                   PrtSetting.Default.SPosX + printableArea.X + (printableArea.Width - scaledWidth) / 2, // Center horizontally
                   PrtSetting.Default.SPosY + printableArea.Y + (printableArea.Height - scaledHeight) / 2, // Center vertically
                    scaledWidth - PrtSetting.Default.BarcodeWidth,
                    scaledHeight - PrtSetting.Default.BarcodeHeight);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Enable anti-aliasing for better quality

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                     e.Command == ApplicationCommands.Cut ||
                     e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
        System.Windows.Threading.DispatcherTimer _typingTimer;
        private void TextPRintBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cancelValidate) // If the input is invalid, do not process further
            {
                cancelValidate = false;
                return;
            }
            if (InputStringBox.Text == "")
            {
                return;
            }
            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(200);

                _typingTimer.Tick += new EventHandler(this.handleTypingTimerTimeout);
            }
            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Tag = (sender as System.Windows.Controls.TextBox).Text; // This should be done with EventArgs
            _typingTimer.Start();
        }
        private ImageSource _BarcodeInsert;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // private ImageSource _BarcodeImage { get; set; }

        //public ImageSource BarcodeImage { get; private set; }
        private ImageSource _BarcodeImage;

        public ImageSource BarcodeImage
        {
            get { return _BarcodeImage; }
            set { _BarcodeImage = value; OnBarcodeChanged(); }
        }

        private void OnBarcodeChanged()
        {
            if (BarcodeImage != null)    
            BarcodeImageChanged.Invoke(BarcodeImage);
        }
        private void RunBarcode(int printtimes=0)
        {
            if (InputBarcodeString == "" || InputBarcodeString == null) { BarcodeLogo = null; return; }
            OutBarcodeString = InputBarcodeString;
            if (PrtSetting.Default.UseQRCode)
            {
                BarcodeLogo = Barcode.GetQRCodeToImageSource(GetBarcodeWithText(printtimes));
            }
            else
            {
                BarcodeLogo = Barcode.BitmapToImageSource(GetBarcodeWithText(printtimes));
            }
            StartRenderImageTimer();
        }
        private string GetBarcodeWithText(int printtimes)
        {
            if(printtimes == 0)
            {
                return OutBarcodeString;
            }
            else
            {
                string oristring = OutBarcodeString;
                OutBarcodeString = $"{OutBarcodeString}-{printtimes}";
                if (!PrtSetting.Default.QRsameWithText)
                {
                    return oristring;
                }
                else
                {

                    return OutBarcodeString; // Append the print times to the barcode string
                }
            }
        }
        private string _InputBarcodeString;

        public string InputBarcodeString
        {
            get { return _InputBarcodeString; }
            set { _InputBarcodeString = value;
                OnPropertyChanged(); }
        }

        private string _OutBarcodeString;

        public string OutBarcodeString
        {
            get { return _OutBarcodeString; }
            set { _OutBarcodeString = value;
                OnPropertyChanged();
            }
        }


        bool cancelValidate = false;
        private void handleTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer; // WPF
            if (timer == null)
            {
                return;
            }
            RunBarcode();
            timer.Stop();
        }
        System.Windows.Threading.DispatcherTimer TimerDelayRendering;
        private void StartRenderImageTimer()
        {
            if (TimerDelayRendering == null)
            {
                TimerDelayRendering = new DispatcherTimer();
                TimerDelayRendering.Interval = TimeSpan.FromMilliseconds(1);
                TimerDelayRendering.Tick += new EventHandler(this.HandleTimerDelay);
            }
            TimerDelayRendering.Stop(); // Resets the timer
            TimerDelayRendering.Start();
        }

        private void HandleTimerDelay(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer; // WPF
            if (timer == null)
            {
                return;
            }
           // InputBarcodeString = OutBarcodeString; 
            BarcodeImage = Barcode.GetRender(InputBarcodeGrid, PrtSetting.Default.PrinterDpi); //Grid to Image
            //OnBarcodeChanged();
            timer.Stop();
        }

        private ImageSource _BarcodeLogo;

        public ImageSource BarcodeLogo
        {
            get { return _BarcodeLogo; }
            set { _BarcodeLogo = value; OnPropertyChanged(); }
        }

        private string verifyInput(string text)
        {
            if(text.Length > 5)
            {
                return text;
            }
            else
            {
                return "";
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public void PrinterSettings_Changed(System.Drawing.Printing.PrinterSettings prtSetting)
        {
            //Barcode View
            try
            {
                BarcodeSize.Width = new GridLength(PrtSetting.Default.BarcodeSize, GridUnitType.Pixel);
                BrTextBloxk.FontSize = PrtSetting.Default.FontSize;
                BrTextBloxk.Margin = new Thickness(PrtSetting.Default.BarcodeTextGap, 0, 0, 0);
                switch (PrtSetting.Default.FontType)
                {
                    case "Normal":
                        BrTextBloxk.FontWeight = FontWeights.Normal;
                        break;
                    case "Bold":
                        BrTextBloxk.FontWeight = FontWeights.Bold;
                        break;
                    default:
                        break;
                }
                BrTextBloxk.FontFamily = new System.Windows.Media.FontFamily(PrtSetting.Default.FontTypeFont);
                Printdocument.PrinterSettings = prtSetting;
                PrinterNameText = "Printer: " + PrtSetting.Default.PrinterName;
                RunBarcode();
            }
            catch (Exception EX)
            {
                System.Windows.Forms.MessageBox.Show("prt Setting Changed" + EX.Message);
            }
        }


        private void PrintBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(InputBarcodeString == "")
            {
                System.Windows.MessageBox.Show("Please enter a valid Lot No.");
                return;
            }
            if(BarcodeImage == null)
            {
                System.Windows.MessageBox.Show("Nothing to Print");
                return;
            }

            InputStringBox.IsEnabled = false;
            try
            {
                Printdocument.PrinterSettings.Copies = 1;
                if (PrtSetting.Default.PrintQty == 0)
                {
                    RunBarcode();
                    Thread.Sleep(100); // Wait for the barcode to be generated
                    Printdocument.Print();
                }
                else if (PrtSetting.Default.PrintQty > 0)
                {
                        for (int i = 1; i <= PrtSetting.Default.PrintQty; i++)
                        {
                            RunBarcode(i);
                            Thread.Sleep(100); // Wait for the barcode to be generated
                            Printdocument.Print();
                        }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            InputStringBox.IsEnabled = true;
        }

        private void AddPrintQty(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if(int.Parse(QtyBox.Text) >= 30)
                {
                    System.Windows.MessageBox.Show("Maximmum label can print is 30");
                    return;
                }
                AddRedQty(false);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ReducePrintQty(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(QtyBox.Text) < 1)
                {
                    System.Windows.MessageBox.Show("Please enter a valid quantity greater than 0.");
                    return;
                }
                AddRedQty(true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void LoadPrintQty()
        {
            QtyBox.Text = PrtSetting.Default.PrintQty.ToString();
        }
        private void AddRedQty(bool isReducing)
        {
            try
            {
                short qty = short.Parse(QtyBox.Text);
                if (isReducing)
                {
                    qty = (short)(qty - 1);
                }
                else
                {
                    qty = (short)(qty + 1);
                }
                QtyBox.Text = qty.ToString();

                PrtSetting.Default.PrintQty = qty;
                PrtSetting.Default.Save(); // Save the updated quantity to settings
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void PrintPrevieww()
        {
            PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();
            printPrvDlg.Document = Printdocument;
            printPrvDlg.ShowDialog(); // this shows the preview and then show the Printer Dlg below
        }

    }
}
