using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class WinApiHelper
    {
        private static IntPtr _prevPoint;
        private static string _prevProcess = string.Empty;
        private static readonly ushort WidthCenter = (ushort)(System.Windows.SystemParameters.WorkArea.Width / 2);
        private static readonly ushort HeightCenter = (ushort)(System.Windows.SystemParameters.WorkArea.Height / 2);
        private static readonly ushort WidthOffset = (ushort)(WidthCenter / 10);
        private static readonly ushort HeightOffset = (ushort)(HeightCenter / 10);


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
            var res = Math.Abs(pos.X - WidthCenter) < WidthOffset && Math.Abs(pos.Y - HeightCenter) < HeightOffset;
            return res;
        }
    }
}