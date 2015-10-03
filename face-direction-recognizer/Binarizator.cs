using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer
{
    class Binarizator : IFilter
    {
        public void DoFilter(FastBitmap bitmap)
        {
            int[] gistogramm = CountGistogram(bitmap);

            int threshold = CountThreshold(gistogramm);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap[i, j] = (byte)(bitmap[i, j] < threshold ? 0: 255);
                }
            }
        }

        private int[] CountGistogram(FastBitmap bitmap)
        {
            int[] result = new int[256];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    result[bitmap[i, j]]++;
                }
            }
            return result;
        }

        private int CountThreshold(int[] gistogramm)
        {
            int result = 0;
            float maxSigma = -1;
            int m = 0, pixelsCount = 0;
            for (int t = 0; t < 128; t++)
            {
                m += t * gistogramm[t];
                pixelsCount += gistogramm[t];
            }

            int alpha1 = 0, beta1 = 0;
            Parallel.For(0, 128, t =>
            {
                alpha1 += t * gistogramm[t];
                beta1 += gistogramm[t];

                float p1 = (float)beta1 / pixelsCount;

                float difference = (float)alpha1 / beta1 - (float)(m - alpha1) / (pixelsCount - beta1);

                float sigma = p1 * (1 - p1) * difference * difference;

                if (sigma > maxSigma)
                {
                    maxSigma = sigma;
                    result = t;
                }
            });
            return result;
        }
    }
}
