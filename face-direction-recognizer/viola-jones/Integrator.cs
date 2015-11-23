using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer.viola_jones
{
    class Integrator
    {
        public int[,] IntegralView
        {
            get;
            private set;
        }

        public int[,] SquareIntegralView
        {
            get;
            private set;
        }

        public Integrator(FastBitmap bitmap)
        {
            IntegralView = new int[bitmap.Width, bitmap.Height];
            SquareIntegralView = new int[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    IntegralView[x, y] = bitmap[x, y];
                    SquareIntegralView[x, y] = bitmap[x, y]*bitmap[x, y];
                    if (x-1 >=0)
                    {
                        IntegralView[x, y] += IntegralView[x - 1, y];
                        SquareIntegralView[x, y] += SquareIntegralView[x - 1, y];
                    }
                    if (y-1 >= 0)
                    {
                        IntegralView[x, y] += IntegralView[x, y -1];
                        SquareIntegralView[x, y] += SquareIntegralView[x, y - 1];
                    }
                    if (x - 1 >= 0 && y - 1 >=0)
                    {
                        IntegralView[x, y] -= IntegralView[x - 1, y - 1];
                        SquareIntegralView[x, y] -= SquareIntegralView[x - 1, y - 1];
                    }
                }
            }
        }
    }
}
