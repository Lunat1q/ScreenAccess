using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition.Logger
{
    internal class DebugLogger
    {
        private readonly string _folder;
        private string _currentSnapshotPath;
        private int _snapShotId = 1;
        private int _snapShotItem;

        public DebugLogger(string folder)
        {
            this._folder = Path.Combine(folder, "DebugSnapshots", DateTime.UtcNow.ToString("dd_MM_yy-HH_mm"));
        }

        public void NewSnapshot()
        {
            this._snapShotItem = 0;
            this._currentSnapshotPath = Path.Combine(this._folder, $"{this._snapShotId++}");
        }

        private void InitPath()
        {
            if (!Directory.Exists(this._currentSnapshotPath))
            {
                Directory.CreateDirectory(this._currentSnapshotPath);
            }
        }

        public void SaveImage(Image img, string suffix = "")
        {
            this.InitPath();
            img.Save(Path.Combine(this._currentSnapshotPath, $"{++this._snapShotItem}{(string.IsNullOrEmpty(suffix) ? "" : "_" + suffix)}.tif"), ImageFormat.Tiff);
        }

        public void SaveRecognitionInfo(string text)
        {
            this.InitPath();
            File.WriteAllText(Path.Combine(this._currentSnapshotPath, $"{this._snapShotItem}.txt"), text, Encoding.UTF8);
        }
    }
}
