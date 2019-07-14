using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.Core
{
    internal static class WinApiHelper
    {
        private static IntPtr _prevPoint;
        private static string _prevProcess = string.Empty;
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
    }
}