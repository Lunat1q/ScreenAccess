using System;
using System.Collections.Generic;
using System.Drawing;
using IronOcr;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends
{
    public class ApLegWeaponTypeScreenRecognizer : IWeaponRecognizer
    {

        private static readonly Dictionary<int, string> ResultCache = new Dictionary<int, string>();

        public bool IsFirstWeaponActive()
        {
            var image1 = ScreenCapture.CaptureScreenRelatively(82f, 84f, 95.5f, 96);
            var image2 = ScreenCapture.CaptureScreenRelatively(90f, 92f, 95.5f, 96);

            //image1.SaveTestImage();
            //image2.SaveTestImage();

            return image1.GetAverageBrightness() > image2.GetAverageBrightness();
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

        private static DirectBitmap GetAdjustedDirectBitmapOfImage(Image img)
        {
            var db = new DirectBitmap(img.Width, img.Height);

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
                        db.SetPixel(i, j, Color.Black);
                    }
                }
            }

            return db;
        }

        public string TestWeapons()
        {
            var w1Img = GetWeapon1Image();
            var w2Img = GetWeapon2Image();
            w1Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w1Img))
            {
                db.ToBitmap().SaveTestImage();
            }

            w2Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w1Img))
            {
                db.ToBitmap().SaveTestImage();
            }

            return WeaponImageToString(w1Img) + WeaponImageToString(w2Img);
        }

        public int GetActiveWeapon()
        {
            return IsFirstWeaponActive() ? 1 : 2;
        }

        public string GetWeaponFromScreen(int no)
        {
            switch (no)
            {
                case 1:
                    return GetWeapon1FromScreen();
                case 2:
                    return GetWeapon2FromScreen();
                default:
                    throw new ArgumentOutOfRangeException(nameof(no));
            }
        }

        private static string GetWeapon1FromScreen()
        {
            return WeaponImageToString(GetWeapon1Image());
        }

        private static string GetWeapon2FromScreen()
        {
            return WeaponImageToString(GetWeapon2Image());
        }

        private static string WeaponImageToString(Image img)
        {
            var result = "";
            try
            {
                using (var db = GetAdjustedDirectBitmapOfImage(img))
                {
                    if (ResultCache.TryGetValue(db.GetHashCode(), out result))
                    {
                        return result;
                    }

                    var img2 = db.ToBitmap();
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
                    result = res.Text.ToUpper();
                    ResultCache.Add(db.GetHashCode(), result);
                }
            }
            catch
            {
                //ignore
            }

            return result;
        }

        public WeaponModuleType[] GetModulesState(int weaponNumberOfModules)
        {
            var ret = new WeaponModuleType[weaponNumberOfModules];
            for (var i = 0; i < weaponNumberOfModules; i++)
            {
                var offsetX = i * 1.45f;
                var image = ScreenCapture.CaptureScreenRelatively(79.25f + offsetX, 80.5f + offsetX, 92.6f, 94.7f);
                //image.SaveTestImage(i.ToString());
                var avColor = image.GetAverageColor();

                if (avColor.R > 100 && avColor.R < 150 &&
                    avColor.G > 85 && avColor.G < 110 &&
                    avColor.B > 125 && avColor.B < 170)
                {
                    ret[i] = WeaponModuleType.Epic;
                }
                else if (avColor.R > 125 && avColor.R < 150 &&
                    avColor.G > 105 && avColor.G < 130 &&
                    avColor.B > 50 && avColor.B < 80)
                {
                    ret[i] = WeaponModuleType.Legendary;
                }
                else if (avColor.R > 45 && avColor.R < 75 &&
                         avColor.G > 80 && avColor.G < 110 &&
                         avColor.B > 115 && avColor.B < 150)
                {
                    ret[i] = WeaponModuleType.Rare;
                }
                else if (avColor.R > 85 && avColor.R < 105 &&
                         avColor.G > 85 && avColor.G < 105 &&
                         avColor.B > 85 && avColor.B < 105)
                {
                    ret[i] = WeaponModuleType.Common;
                }
                else
                {
                    ret[i] = WeaponModuleType.None;
                }
            }

            return ret;
        }
    }

    public interface IWeaponRecognizer
    {
        string GetWeaponFromScreen(int no);

        string TestWeapons();

        int GetActiveWeapon();
    }
}