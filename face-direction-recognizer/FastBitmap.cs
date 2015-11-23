using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace face_direction_recognizer
{
    class FastBitmap
    {
        private static ColorPalette _palette;
        private int[,] _integralImage;

        static FastBitmap()
        {
            Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            _palette = bmp.Palette;
            for (int i = 0; i <= 255; i++)
            {
                _palette.Entries[i] = Color.FromArgb(i, i, i);
            }
        }

        private Bitmap _bitmap;
        private byte[] _grayPixels;

        public int Height
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public Bitmap GrayBitmap
        {
            get
            {
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
                bitmap.Palette = _palette;
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                Marshal.Copy(_grayPixels, 0, data.Scan0, _grayPixels.Length);
                bitmap.UnlockBits(data);
                return bitmap;
            }
        }

        public byte this[int x, int y]
        {
            get
            {
                return _grayPixels[y * Width + x];
            }
            set
            {
                _grayPixels[y * Width + x] = value;
            }
        }

        public FastBitmap(Bitmap bitmap)
        {
            _bitmap = bitmap;
            _grayPixels = ToGrayscale(_bitmap);
            Height = _bitmap.Height;
            Width = _bitmap.Width;
            _integralImage = ToIntegral(_bitmap);
        }

        private int[,] ToIntegral(Bitmap bitmap)
        {
            int[,] result = new int[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    
                }
            }
            return result;
        }

        private byte[] ToGrayscale(Bitmap bitmap)
        {
            int bytesPerChannel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData colorData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr colorPointer = colorData.Scan0;
            byte[] pixels = new byte[colorData.Height * colorData.Width * bytesPerChannel];
            byte[] grayPixels = new byte[bitmap.Width * bitmap.Height];
            Marshal.Copy(colorPointer, pixels, 0, pixels.Length);
            bitmap.UnlockBits(colorData);
            int j = 0;
            for (int i = 0; i < pixels.Length; i += bytesPerChannel)
            {
                grayPixels[j++] = (byte)(0.299 * pixels[i + 2] + 0.587 * pixels[i + 1] + 0.114 * pixels[i]);
            }
            return grayPixels;
        }
    }
}
