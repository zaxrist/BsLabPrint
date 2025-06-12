using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class UserControlToImageConverter
{
    public static BitmapImage ConvertUserControlToImageAsync(FrameworkElement userControl)
    {
            // Measure and Arrange the UserControl
            userControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            userControl.Arrange(new Rect(new Size(userControl.DesiredSize.Width, userControl.DesiredSize.Height)));

            // Create DrawingVisual
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                // Draw the UserControl to the DrawingContext
                dc.DrawRectangle(new VisualBrush(userControl), null, new Rect(new Point(0, 0), new Size(userControl.DesiredSize.Width, userControl.DesiredSize.Height)));
            }

            // Create RenderTargetBitmap
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)userControl.DesiredSize.Width, (int)userControl.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            // Convert to BitmapImage
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            using (MemoryStream ms = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(ms);
                ms.Position = 0;
                image.StreamSource = ms;
                image.EndInit();
            }

            return image;

    }

    //public static async Task<bool> SaveUserControlToImageFileAsync(FrameworkElement userControl, string filePath)
    //{
    //    try
    //    {
    //        // Convert to BitmapImage (using the same logic as in ConvertUserControlToImageAsync)
    //        // ... (omit the code for brevity) ...
    //        BitmapImage image = await ConvertUserControlToImageAsync(userControl);

    //        // Save to file
    //        using (FileStream fs = new FileStream(filePath, FileMode.Create))
    //        {
    //            PngBitmapEncoder encoder = new PngBitmapEncoder();
    //            encoder.Frames.Add(BitmapFrame.Create(image));
    //            encoder.Save(fs);
    //        }
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}
}