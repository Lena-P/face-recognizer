using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer
{
    class SobelDetektor : IFilter
    {
        public void DoFilter(FastBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[,] windowX = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] windowY = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            byte[,] buffer = new byte[width, height];

            Parallel.For(1, width - 1, i =>
              {
                  for (int j = 1; j < height - 1; j++)
                  {
                      int new_x = 0;
                      int new_y = 0;

                      for (int wi = -1; wi < 2; wi++)
                      {
                          for (int hj = -1; hj < 2; hj++)
                          {
                              new_x += windowX[wi + 1, hj + 1] * bitmap[i + hj, j + wi];
                              new_y += windowY[wi + 1, hj + 1] * bitmap[i + hj, j + wi];
                          }
                      }
                      double new_val = Math.Sqrt(new_x * new_x + new_y * new_y);
                      if (new_val > 255) new_val = 255;
                      if (new_val < 0) new_val = 0;
                      buffer[i, j] = (byte)new_val;
                  }
              });
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                   bitmap[i,j] = buffer[i, j];
                }
            }
        }
    }
}
