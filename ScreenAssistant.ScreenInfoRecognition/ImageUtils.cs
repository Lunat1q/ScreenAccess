using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    internal static class ImageUtils
    {
        private static readonly Random Rnd = new Random();

        internal static float GetAverageBrightness(this Image img)
        {
            var brightnessTotal = 0f;

            if (img.Height == 0 || img.Width == 0)
            {
                return 0;
            }
            using (var db = new DirectBitmap(img.Width, img.Height))
            {
                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                for (var i = 0; i < img.Width; i++)
                {
                    for (var j = 0; j < img.Height; j++)
                    {
                        var currentPixel = db.GetPixel(i, j);
                        brightnessTotal = currentPixel.GetBrightness();
                    }
                }
            }

            return brightnessTotal / (img.Width * img.Height);
        }

        internal static Color GetAverageColor(this Image img)
        {
            using (var db = new DirectBitmap(img.Width, img.Height))
            {
                using (var graphics = Graphics.FromImage(db.Bitmap))
                {
                    graphics.DrawImage(img, Point.Empty);
                }

                return db.GetDominantColor();
            }
        }

        internal static void SaveTestImage(this Image image, string prefix  = "")
        {
            var fileName = $"{Rnd.Next(0, 999999999)}.tif";
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                fileName = $"{prefix}_{fileName}";
            }
            var dir = Directory.CreateDirectory("TestSC");
            image.Save(Path.Combine(dir.FullName, fileName), ImageFormat.Tiff);
        }
    }
}