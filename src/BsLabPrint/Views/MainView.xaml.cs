using BsLabPrint.Modules;
using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Reflection.Emit;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace BsLabPrint.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

           // BarcodeInsert.Source = Barcode.BitmapToImageSource("1234567890123");
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
            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(1000);

                _typingTimer.Tick += new EventHandler(this.handleTypingTimerTimeout);
            }
            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Tag = (sender as TextBox).Text; // This should be done with EventArgs
            _typingTimer.Start();
        }
        private void handleTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer; // WPF
            if (timer == null)
            {
                return;
            }

            string validateString = verifyInput(timer.Tag as string);
            TextPRintBox.Text = validateString;
            TextInput.Text = validateString;
            if(validateString != "")
            {
                BarcodeInsert.Source = Barcode.BitmapToImageSource(validateString);
                PrintPreview.Source= BitmapToImageSource( GridToImage());
                TextPRintBox.SelectAll();
            }

            timer.Stop();
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
        private Bitmap GridToImage()
        {
            RenderTargetBitmap rnd = new RenderTargetBitmap((int)InputBarcode.ActualWidth, (int)InputBarcode.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            rnd.Render(InputBarcode);
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rnd));
            encoder.Save(stream);
            Bitmap bitmap = new Bitmap(stream);
            return bitmap;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
