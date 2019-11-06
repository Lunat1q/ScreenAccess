﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    public static class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public static Image CaptureScreen()
        {
            return CaptureScreenRelatively(0, 100, 0, 100, true);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of the desktop part
        /// </summary>
        /// <param name="startX">0-100 percent of start by X</param>
        /// <param name="endX">0-100 percent of end by X</param>
        /// <param name="startY">0-100 percent of start by Y</param>
        /// <param name="endY">0-100 percent of end by Y</param>
        /// <param name="fullScreen">FullScreen capture mode</param>
        /// <returns></returns>
        public static Image CaptureScreenRelatively(float startX, float endX, float startY, float endY, bool fullScreen)
        {
            if (fullScreen)
            {
                return CaptureWindow(User32.GetDesktopWindow(), startX, endX, startY, endY);
                //return CaptureWindowFromGraphics(User32.GetForegroundWindow(), startX, endX, startY, endY);
            }

            return CaptureWindow(User32.GetForegroundWindow(), startX, endX, startY, endY);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <param name="startX">0-100 percent of start by X</param>
        /// <param name="endX">0-100 percent of end by X</param>
        /// <param name="startY">0-100 percent of start by Y</param>
        /// <param name="endY">0-100 percent of end by Y</param>
        /// <returns></returns>
        internal static Image CaptureWindow(IntPtr handle, float startX, float endX, float startY, float endY)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, (int)((endX - startX) * width / 100), (int)((endY - startY) * height / 100));
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, (int)((endX - startX) * width / 100), (int)((endY - startY) * height / 100), 
                hdcSrc, (int)(startX * width / 100), (int)(startY * height / 100), GDI32.SRCCOPY | GDI32.CAPTUREBLT);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handlePtr">The handle to the window.</param>
        /// <param name="startX">0-100 percent of start by X</param>
        /// <param name="endX">0-100 percent of end by X</param>
        /// <param name="startY">0-100 percent of start by Y</param>
        /// <param name="endY">0-100 percent of end by Y</param>
        /// <returns></returns>
        internal static Image CaptureWindowFromGraphics(IntPtr handlePtr, float startX, float endX, float startY, float endY)
        {
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handlePtr, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            var tWidth = (int) ((endX - startX) * width / 100);
            var tHeight = (int) ((endY - startY) * height / 100);
            var xSrc = (int) (startX * width / 100);
            var ySrc = (int) (startY * height / 100);

            if (tWidth > 0 && tHeight > 0)
            {
                Bitmap bitmap = new Bitmap(tWidth, tHeight);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(xSrc, ySrc), Point.Empty, new Size(tWidth, tHeight));
                }
                return bitmap;
            }

            return new Bitmap(1, 1);
        }


        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle, 0, 100, 0, 100);
            img.Save(filename, format);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public static void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {
            public const int CAPTUREBLT = 0x40000000;

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        public class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
        }
    }
}