using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using IronOcr;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    public static class WeaponTypeScreenRecognizer
    {
        private static readonly Dictionary<string, WeaponAL> WeaponNamesToTypes = new Dictionary<string, WeaponAL>(35);
        private static readonly Random Rnd = new Random();
        static WeaponTypeScreenRecognizer()
        {
            var values = Enum.GetValues(typeof(WeaponAL)).Cast<Enum>();
            foreach (var value in values)
            {
                if (value.GetType().GetField(value.ToString())
                    .GetCustomAttributes(typeof(WeaponNameAttribute), false).FirstOrDefault() is WeaponNameAttribute nameAttribute)
                {
                    WeaponNamesToTypes.Add(nameAttribute.Name.ToUpper(), (WeaponAL)value);
                    foreach (var extraName in nameAttribute.ExtraRecognitionNames)
                    {
                        WeaponNamesToTypes.Add(extraName.ToUpper(), (WeaponAL)value);
                    }
                }
            }
        }

        public static bool IsFirstWeaponActive()
        {
            var image1 = ScreenCapture.CaptureScreenRelatively(82f, 84f, 95.5f, 96);
            var image2 = ScreenCapture.CaptureScreenRelatively(90f, 92f, 95.5f, 96);

            //image1.SaveTestImage();
            //image2.SaveTestImage();

            return image1.GetAverageBrightness() > image2.GetAverageBrightness();
        }

        private static float GetAverageBrightness(this Image img)
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

        private static Image GetWeapon1Image()
        {
            var image = ScreenCapture.CaptureScreenRelatively(81f, 87f, 95.5f, 98);//77, 97, 95, 99);
            return image;
        }

        private static Image GetWeapon2Image()
        {
            var image = ScreenCapture.CaptureScreenRelatively(88.3f, 95f, 95.5f, 98);//77, 97, 95, 99);
            return image;
        }

        private static void SaveTestImage(this Image image)
        {
            var fileName = $"{Rnd.Next(0, 999999999)}.tif";
            Directory.CreateDirectory("TestSC");
            image.Save(@"\TestSC\" + fileName, ImageFormat.Tiff);
        }

        private static Image AdjustImageForRecognition(Image img)
        {
            Image result;
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
                        var brightness = currentPixel.GetBrightness();
                        if (brightness < 0.85f)
                        {
                            db.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            db.SetPixel(i,j, Color.Black);
                        }
                    }
                }

                result = (Bitmap) db.Bitmap.Clone();
            }

            return result;
        }

        public static WeaponAL TestWeapons()
        {
            var w1Img = GetWeapon1Image();
            var w2Img = GetWeapon2Image();
            SaveTestImage(w1Img);
            SaveTestImage(AdjustImageForRecognition(w1Img));
            SaveTestImage(w2Img);
            SaveTestImage(AdjustImageForRecognition(w2Img));
            return WeaponImageToType(w1Img) | WeaponImageToType(w2Img);
        }

        public static WeaponAL GetWeapon1FromScreen()
        {
            return WeaponImageToType(GetWeapon1Image());
        }

        public static WeaponAL GetWeapon2FromScreen()
        {
            return WeaponImageToType(GetWeapon2Image());
        }

        private static WeaponAL WeaponImageToType(Image img)
        {
            var result = WeaponAL.Unknown;
            try
            {
                var img2 = AdjustImageForRecognition(img);
                var ocr = new AdvancedOcr
                {
                    CleanBackgroundNoise = true,
                    EnhanceContrast = true,
                    EnhanceResolution = true,
                    Language = IronOcr.Languages.English.OcrLanguagePack,
                    Strategy = AdvancedOcr.OcrStrategy.Advanced,
                    ColorSpace = AdvancedOcr.OcrColorSpace.GrayScale,
                    DetectWhiteTextOnDarkBackgrounds = false,
                    InputImageType = AdvancedOcr.InputTypes.Snippet,
                    RotateAndStraighten = false,
                    ReadBarCodes = false,
                    ColorDepth = 8
                };
                var res = ocr.Read(img2);

                var name = FindMostSimilar(WeaponNamesToTypes.Keys, res.Text);

                WeaponNamesToTypes.TryGetValue(name, out result);
            }
            catch
            {
                //ignore
            }

            return result;
        }

        public static string FindMostSimilar(IEnumerable<string> words, string searchFor)
        {
            return words.Aggregate((x, v) => Compute(x, searchFor) < Compute(v, searchFor) ? x : v);
        }

        private static int Compute(string s, string t)
        {
            int n = s.Length, m = t.Length;
            var d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;


            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (var i = 1; i <= n; i++)
            for (var j = 1; j <= m; j++)
            {
                var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
            return d[n, m];
        }
    }
}