﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace TiqSoft.ScreenAssistant.ScreenInfoRecognition
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        public int[] Bits { get; }
        public bool Disposed { get; private set; }
        public int Height { get; }
        public int Width { get; }

        protected GCHandle BitsHandle { get; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = x + (y * Width);
            int col = color.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        internal Bitmap ToBitmap()
        {
            return (Bitmap)Bitmap.Clone();
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        public Color GetDominantColor()
        {
            long[] total = {0, 0, 0};
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    int index = i + j * Width;
                    var color = Color.FromArgb(Bits[index]);
                    total[0] += color.R;
                    total[1] += color.G;
                    total[2] += color.B;
                }
            }

            total[0] /= Width * Height;
            total[1] /= Width * Height;
            total[2] /= Width * Height;
            return Color.FromArgb((int)total[0], (int)total[1], (int)total[2]);
        }

        public override int GetHashCode()
        {
            return Bits.Aggregate(Bits.Length, (current, t) => unchecked(current * 31 + t));
        }
    }
}