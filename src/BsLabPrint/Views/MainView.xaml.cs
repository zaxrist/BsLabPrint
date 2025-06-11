using BsLabPrint.Modules;
using BsLabPrint.PrinterSetting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private void OnBarcodeImageChanged()
        {
            throw new NotImplementedException();
        }

        private void Printdocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                

                Graphics gp = e.Graphics;


                Bitmap bb = Barcode.ImageSourceToBitmap(BarcodeImage);

                // gp.DrawImage(bb, new Point(PrtSetting.Default.SPosX, PrtSetting.Default.SPosY),float.Parse(PrtSetting.Default.LabelWidth),PrtSetting.Default.LabelHeight);
                gp.DrawImage(bb ,PrtSetting.Default.SPosX, PrtSetting.Default.SPosY, PrtSetting.Default.BarcodeWidth, PrtSetting.Default.BarcodeHeight);
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
            if(InputBarcodeString == "")
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

        private ImageSource _BarcodeImage;

        public ImageSource BarcodeImage
        {
            get { return _BarcodeImage; }
            set { _BarcodeImage = value; OnPropertyChanged(); OnBarcodeChanged(); }
        }


        private void OnBarcodeChanged()
        {
            if (BarcodeImage != null)
                BarcodeImageChanged.Invoke(BarcodeImage);
        }

        private string _InputBarcodeString;

        public string InputBarcodeString
        {
            get { return _InputBarcodeString; }
            set { _InputBarcodeString = value; 
                if(InputBarcodeString!="") BarcodeLogo = Barcode.BitmapToImageSource(InputBarcodeString); 
                else BarcodeLogo = null;
                OnPropertyChanged(); }
        }


        private void handleTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer; // WPF
            if (timer == null)
            {
                return;
            }
            //System.Windows.MessageBox.Show("Test");
            RunBarcode();
            TextPRintBox.SelectAll();
            timer.Stop();
        }

        private ImageSource _BarcodeLogo;

        public ImageSource BarcodeLogo
        {
            get { return _BarcodeLogo; }
            set { _BarcodeLogo = value; OnPropertyChanged(); }
        }


        private void RunBarcode()
        {
            //import to grid
            BarcodeImage = Barcode.GetRender(InputBarcodeGrid, 96); //Grid to Image
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
            //System.Windows.MessageBox.Show("setting Changed");
            Printdocument.PrinterSettings = prtSetting;
        }

        private void PrintBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Printdocument.PrinterSettings.Copies = PrtSetting.Default.PrintQty; // Set the number of copies to print
                Printdocument.Print();

                //PrintPrevieww();
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
            
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
                if (int.Parse(QtyBox.Text) <= 1)
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
