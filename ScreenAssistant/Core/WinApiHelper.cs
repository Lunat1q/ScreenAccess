using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqUtils.Wpf.Screen;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class WinApiHelper
    {
        private static IntPtr _prevPoint;
        private static string _prevProcess = string.Empty;
        private static Dictionary<IntPtr, WindowInfo> _windowsCache = new Dictionary<IntPtr, WindowInfo>(); 


        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        internal static string GetActiveProcessName()
        {
            var fWindow = GetForegroundWindow();
            if (fWindow != _prevPoint)
            {
                _prevPoint = fWindow;
                GetWindowThreadProcessId(fWindow, out var pid);
                var p = Process.GetProcessById((int)pid);
                _prevProcess = p.ProcessName;
            }
            return _prevProcess;
        }

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PointStruct
        {
            public int X;
            public int Y;

            public override string ToString()
            {
                return $"x:{this.X} y:{this.Y}";
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointStruct lpPoint);

        public static PointStruct GetCursorPosition()
        {
            GetCursorPos(out var lpPoint);
            return lpPoint;
        }

        public static bool IsCursorAtTheCenter()
        {
            var pos = GetCursorPosition();
            var currentWindowId = GetForegroundWindow();
            if (!_windowsCache.TryGetValue(currentWindowId, out var windowInfo))
            {
                var windowRect = new ScreenCapture.User32.RECT();
                ScreenCapture.User32.GetWindowRect(currentWindowId, ref windowRect);
                windowInfo = new WindowInfo(windowRect); //, (ushort)DpiHelper.GetDpiForWindow(currentWindowId));
                _windowsCache.Add(currentWindowId, windowInfo);
            }

            return windowInfo.IsCursorAtTheCenter(pos);
        }

        private class WindowInfo // DPI was not required after all
        {
            private const int OFFSET_FRACTION = 20;
            // private const ushort DEFAULT_DPI = 96;

            private readonly ushort _offsetLimitX;
            private readonly ushort _offsetLimitY;
            private readonly ushort _windowCenterX;
            private readonly ushort _windowCenterY;
            // private readonly ushort _dpi;

            public WindowInfo(ScreenCapture.User32.RECT rect) //, ushort dpi)
            {
                var height = rect.bottom - rect.top;
                var width = rect.right - rect.left;
                this._offsetLimitX = (ushort)(width / OFFSET_FRACTION);
                this._offsetLimitY = (ushort)(height / OFFSET_FRACTION);
                this._windowCenterX = (ushort)(rect.left + width / 2);
                this._windowCenterY = (ushort)(rect.top + height / 2);
                //this._dpi = dpi;
            }

            internal bool IsCursorAtTheCenter(PointStruct cursorPos)
            {
                return Math.Abs(cursorPos.X - this._windowCenterX) < this._offsetLimitX && Math.Abs(cursorPos.Y - this._windowCenterY) < this._offsetLimitY;
            }
        }
    }
}