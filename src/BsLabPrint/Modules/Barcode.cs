using DataMatrix.net;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

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

        public static ImageSource GetRender(this UIElement source, double dpi)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(source);

            var scale = dpi / 96.0;
            var width = (bounds.Width + bounds.X) * scale;
            var height = (bounds.Height + bounds.Y) * scale;

            RenderTargetBitmap rtb =
                new RenderTargetBitmap((int)Math.Round(width, MidpointRounding.AwayFromZero),
                (int)Math.Round(height, MidpointRounding.AwayFromZero),
                dpi, dpi, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(source);
                ctx.DrawRectangle(vb, null,
                    new Rect(new System.Windows.Point(bounds.X, bounds.Y), new System.Windows.Point(width, height)));
            }

            rtb.Render(dv);
            return (ImageSource)rtb.GetAsFrozen();
        }
    }
}
