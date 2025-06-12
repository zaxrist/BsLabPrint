using DataMatrix.net;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using QRCoder;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace BsLabPrint.Modules
{
    static class Barcode
    {

        public static BitmapImage BitmapToImageSource(string text)
        {
            if (text == "") { throw new ArgumentException("Text cannot be empty"); }
            Bitmap bb = new Bitmap(1000, 1000);
            bb = new DmtxImageEncoder().EncodeImage(text);
            using (MemoryStream memory = new MemoryStream())
            {
                bb.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static ImageSource GetRender(this UIElement source, double dpi) //render user control object
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(source);

            //Bounds: X: 0, Y: 0,Width: 450.37, Heigh: 80.4933333333333
            var scale = dpi / 96.0;
            var width = (bounds.Width + bounds.X) * scale;
            var height = (bounds.Height + bounds.Y) * scale;

            Debug.Print($"Bounds: X: {bounds.X}, Y: {bounds.Y},Width: {bounds.Width}, Heigh: {bounds.Height}");

            RenderTargetBitmap rtb =
                new RenderTargetBitmap((int)Math.Round(width, MidpointRounding.AwayFromZero),
                (int)Math.Round(height, MidpointRounding.AwayFromZero),
                dpi, dpi, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(source);
                ctx.DrawRectangle(vb, null,
                    new Rect(new System.Windows.Point(bounds.X, bounds.Y), new System.Windows.Point(bounds.Width, bounds.Height)));
            }

            rtb.Render(dv);
            return (ImageSource)rtb.GetAsFrozen();
        }

        public static Bitmap ImageSourceToBitmap(ImageSource source)
        {
            // Check if the source is a BitmapSource (or can be converted to one)
            if (source is BitmapSource bitmapSource)
            {
                // Use the StreamSource to create a Bitmap
                using (var stream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder(); // Or any other format
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                    stream.Position = 0;

                    // Create the Bitmap from the stream
                    return new Bitmap(stream);
                }
            }
            else
            {
                // Handle other ImageSource types if needed.  For example, a UriImageSource
                // can be loaded directly into a BitmapImage and then converted.
                throw new NotSupportedException("ImageSource type not supported.");
            }
        }

        public static BitmapImage GetQRCodeToImageSource(string QRText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRText, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            //From here on, you can implement your platform-dependent byte[]-to-image code 

            //e.g. Windows 10 - Full .NET Framework
            Bitmap bmp;
            using (var ms = new MemoryStream(qrCodeAsBitmapByteArr))
            {
                 bmp = new Bitmap(ms);

                using (MemoryStream memory = new MemoryStream())
                {
                    bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
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
}
