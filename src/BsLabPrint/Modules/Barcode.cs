using DataMatrix.net;
using System;
using System.Drawing;
using System.IO;
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
    }
}
