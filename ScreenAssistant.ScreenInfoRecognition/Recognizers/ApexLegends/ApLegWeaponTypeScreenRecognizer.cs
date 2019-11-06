using System;
using System.Drawing;
using System.Threading.Tasks;
using IronOcr;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Logger;
using TiqUtils.TypeSpecific;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends
{
    public class ApLegWeaponTypeScreenRecognizer : IWeaponRecognizer
    {
        private const string OcrCacheFile = "OCRCache.json";
        private static readonly SmartLimitedCache<int, string> ResultCache = SmartLimitedCache<int, string>.RestoreFromFile(OcrCacheFile, 100);
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
            this._logger = new DebugLogger(AppDomain.CurrentDomain.BaseDirectory);
#endif
        }

        public bool IsFirstWeaponActive()
        {
            var image1 = ScreenCapture.CaptureScreenRelatively(82f, 84f, 95.5f, 96, this.FullScreenMode);
            var image2 = ScreenCapture.CaptureScreenRelatively(90f, 92f, 95.5f, 96, this.FullScreenMode);
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

        public async Task<string> TestWeapons()
        {
            this._logger?.NewSnapshot();
            var w1Img = GetWeapon1Image(this.FullScreenMode);
            var w2Img = GetWeapon2Image(this.FullScreenMode);
            w1Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w1Img, this._brightnessAdj))
            {
                db.ToBitmap().SaveTestImage();
            }

            w2Img.SaveTestImage();
            using (var db = GetAdjustedDirectBitmapOfImage(w2Img, this._brightnessAdj))
            {
                db.ToBitmap().SaveTestImage();
            }

            return (await this.WeaponImageToString(w1Img)) + (await this.WeaponImageToString(w2Img));
        }

        public int GetActiveWeapon()
        {
            return this.IsFirstWeaponActive() ? 1 : 2;
        }

        public bool FullScreenMode { get; set; }
        public void SetBrightness(float brightnessScale)
        {
            this._brightnessAdj = brightnessScale;
        }

        public async Task<string> GetWeaponFromScreen(int no)
        {
            switch (no)
            {
                case 1:
                    this._logger?.NewSnapshot();
                    return await this.GetWeapon1FromScreen(this.FullScreenMode);
                case 2:
                    return await this.GetWeapon2FromScreen(this.FullScreenMode);
                default:
                    throw new ArgumentOutOfRangeException(nameof(no));
            }
        }

        private async Task<string> GetWeapon1FromScreen(bool fullScreen)
        {
            return await this.WeaponImageToString(GetWeapon1Image(fullScreen));
        }

        private async Task<string> GetWeapon2FromScreen(bool fullScreen)
        {
            return await this.WeaponImageToString(GetWeapon2Image(fullScreen));
        }

        private async Task<string> WeaponImageToString(Image img)
        {
            var result = "";
            try
            {
                using (var db = GetAdjustedDirectBitmapOfImage(img, this._brightnessAdj))
                {
                    var hashCode = db.GetHashCode();
                    if (ResultCache.TryGetValue(hashCode, out result))
                    {
                        if (!result.Empty())
                        {
                            ResultCache.ScoreHit(hashCode);
                        }
                        return result;
                    }

                    this._logger?.SaveImage(img);
                    var img2 = db.ToBitmap();
                    this._logger?.SaveImage(img2, "adj");
                    var pixelsCoefficient = db.GetMeaningfulPixelsCoefficient;
                    if (pixelsCoefficient < 0.6 && pixelsCoefficient > 0.01)
                    {
                        await Task.Delay(1500);
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
                        this._logger?.SaveRecognitionInfo(result);
                    }

                    ResultCache.Add(db.GetHashCode(), result);
                    ResultCache.SaveToFile(OcrCacheFile);
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
                var image = ScreenCapture.CaptureScreenRelatively(79.3f + offsetX, 79.5f + offsetX, 92.8f, 93.2f, this.FullScreenMode);
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
        Task<string> GetWeaponFromScreen(int no);

        Task<string> TestWeapons();

        int GetActiveWeapon();

        bool FullScreenMode { get; set; }

        void SetBrightness(float brightnessScale);
    }
}