using System;
using System.Collections.Generic;
using System.Drawing;
using IronOcr;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Logger;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends
{
    public class ApLegWeaponTypeScreenRecognizer : IWeaponRecognizer
    {

        private static readonly Dictionary<int, string> ResultCache = new Dictionary<int, string>();
        private static readonly Color RareColor = Color.FromArgb(25, 70, 110);
        private static readonly Color Rare2Color = Color.FromArgb(60, 70, 110);
        private static readonly Color EpicColor = Color.FromArgb(80, 40, 115);
        private static readonly Color CommonColor = Color.FromArgb(80, 80, 80);
        private static readonly Color LegendaryColor = Color.FromArgb(100, 80, 10);
#pragma warning disable 649
        private readonly DebugLogger _logger;
#pragma warning restore 649
        private float _brightnessAdj;

        public ApLegWeaponTypeScreenRecognizer()
        {
#if DEBUG
            _logger = new DebugLogger(AppDomain.CurrentDomain.BaseDirectory);
#endif
        }

        public bool IsFirstWeaponActive()
        {
            var image1 = ScreenCapture.CaptureScreenRelatively(82f, 84f, 95.5f, 96, FullScreenMode);
            var image2 = ScreenCapture.CaptureScreenRelatively(90f, 92f, 95.5f, 96, FullScreenMode);
            return image1.GetAverageBrightness() > image2.GetAverageBrightness();
        }

        private static Image GetWeapon1Image(bool fullScreen)
        {
            var image = ScreenCapture.CaptureScreenRelatively(81f, 87f, 95.5f, 98, fullScreen);//77, 97, 95, 99);
            return image;
        }

        private static Image GetWeapon2Image(bool fullScreen)
        {
            var image = ScreenCapture.CaptureScreenRelatively(88.3f, 95f, 95.5f, 98, fullScreen);//77, 97, 95, 99);
            return image;
        }

        private static DirectBitmap GetAdjustedDirectBitmapOfImage(Image img, float brightnessAdj)
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
                    if (brightness < 0.85f * brightnessAdj)
                    {
                        db.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        db.SetPixel(i, j, Color.Black);
                        db.PixelsWithData++;
                    }
                }
            }

            return db;
        }

        public string TestWeapons()
        {
            var w1Img = GetWeapon1Image(FullScreenMode);
            var w2Img = GetWeapon2Image(FullScreenMode);
            w1Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w1Img, _brightnessAdj))
            {
                db.ToBitmap().SaveTestImage();
            }

            w2Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w2Img, _brightnessAdj))
            {
                db.ToBitmap().SaveTestImage();
            }

            return WeaponImageToString(w1Img) + WeaponImageToString(w2Img);
        }

        public int GetActiveWeapon()
        {
            return IsFirstWeaponActive() ? 1 : 2;
        }

        public bool FullScreenMode { get; set; }
        public void SetBrightness(float brightnessScale)
        {
            this._brightnessAdj = brightnessScale;
        }

        public string GetWeaponFromScreen(int no)
        {
            switch (no)
            {
                case 1:
                    _logger?.NewSnapshot();
                    return GetWeapon1FromScreen(FullScreenMode);
                case 2:
                    return GetWeapon2FromScreen(FullScreenMode);
                default:
                    throw new ArgumentOutOfRangeException(nameof(no));
            }
        }

        private string GetWeapon1FromScreen(bool fullScreen)
        {
            return WeaponImageToString(GetWeapon1Image(fullScreen));
        }

        private string GetWeapon2FromScreen(bool fullScreen)
        {
            return WeaponImageToString(GetWeapon2Image(fullScreen));
        }

        private string WeaponImageToString(Image img)
        {
            var result = "";
            try
            {
                using (var db = GetAdjustedDirectBitmapOfImage(img, _brightnessAdj))
                {
                    if (ResultCache.TryGetValue(db.GetHashCode(), out result))
                    {
                        return result;
                    }
                    _logger?.SaveImage(img);
                    var img2 = db.ToBitmap();
                    _logger?.SaveImage(img2, "adj");
                    var pixelsCoefficient = db.GetMeaningfulPixelsCoefficient;
                    if (pixelsCoefficient < 0.7 && pixelsCoefficient > 0.05)
                    {
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
                        _logger?.SaveRecognitionInfo(result);
                    }

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

            var baseColors = new[]
            {
                CommonColor,
                RareColor,
                Rare2Color,
                EpicColor,
                LegendaryColor
            };

            for (var i = 0; i < weaponNumberOfModules; i++)
            {
                var offsetX = i * 1.46f;
                var image = ScreenCapture.CaptureScreenRelatively(79.3f + offsetX, 79.5f + offsetX, 92.8f, 93.2f, FullScreenMode);
                //image.SaveTestImage(i.ToString());
                var avColor = image.GetAverageColor();

                var closestColor = ImageUtils.GetClosestColor(baseColors, avColor);

                if (closestColor == CommonColor)
                {
                    ret[i] = WeaponModuleType.Common;
                }
                else if (closestColor == RareColor || closestColor == Rare2Color)
                {
                    ret[i] = WeaponModuleType.Rare;
                }
                else if (closestColor == EpicColor)
                {
                    ret[i] = WeaponModuleType.Epic;
                }
                else if (closestColor == LegendaryColor)
                {
                    ret[i] = WeaponModuleType.Legendary;
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

        bool FullScreenMode { get; set; }

        void SetBrightness(float brightnessScale);
    }
}