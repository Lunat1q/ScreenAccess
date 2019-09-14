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
            _folder = Path.Combine(folder, "DebugSnapshots", DateTime.UtcNow.ToString("dd_MM_yy-HH_mm"));
        }

        public void NewSnapshot()
        {
            _snapShotItem = 0;
            _currentSnapshotPath = Path.Combine(_folder, $"{_snapShotId++}");
        }

        private void InitPath()
        {
            if (!Directory.Exists(_currentSnapshotPath))
            {
                Directory.CreateDirectory(_currentSnapshotPath);
            }
        }

        public void SaveImage(Image img, string suffix = "")
        {
            InitPath();
            img.Save(Path.Combine(_currentSnapshotPath, $"{++_snapShotItem}{(string.IsNullOrEmpty(suffix) ? "" : "_" + suffix)}.tif"), ImageFormat.Tiff);
        }

        public void SaveRecognitionInfo(string text)
        {
            InitPath();
            File.WriteAllText(Path.Combine(_currentSnapshotPath, $"{_snapShotItem}.txt"), text, Encoding.UTF8);
        }
    }
}
