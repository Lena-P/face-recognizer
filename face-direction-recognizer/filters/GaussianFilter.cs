using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace face_direction_recognizer
{
    class GaussianFilter : IFilter
    {
        private int _sigma;
        private int _size;
        private float[] _coefficients;
        private float _sumCoef;

        public GaussianFilter(int sigma, int size)
        {
            _sigma = sigma;
            _size = size;
            _coefficients = CountCoefficients(_sigma, _size);
        }

        public void DoFilter(FastBitmap bitmap)
        {

            float[,] buffer = new float[bitmap.Width, bitmap.Height];
            Parallel.For(0, bitmap.Height, j =>
            {
                for (int i = _size; i < bitmap.Width - _size; i++)
                {
                    float pix = 0;
                    for (int k = -_size; k < _size; k++)
                    {
                        pix += bitmap[i + k, j] * _coefficients[k + _size];
                    }
                    buffer[i, j] = pix / _sumCoef;
                }
            });

            Parallel.For(0, bitmap.Width, i =>
           {
               for (int j = _size; j < bitmap.Height - _size; j++)
               {
                   float pix = 0;
                   for (int k = -_size; k < _size; k++)
                   {
                       pix += buffer[i, j + k] * _coefficients[k + _size];
                   }
                   bitmap[(int)i, j] = (byte)(pix / _sumCoef);
               }
           });
        }

        private float[] CountCoefficients(int sigma, int size)
        {
            _sumCoef = 0;
            float[] coefficients = new float[size*2 + 1];
            float constBeforeExp = (float)(1 / (Math.Sqrt(2 * Math.PI) * sigma));
            float constExp = -1 / (2 * sigma * sigma);

            for (int i = 0; i < size; i++)
            {
                coefficients[i] = (float)(constBeforeExp * Math.Exp(constExp * (i - _size) * (i - _size)));
            _sumCoef += coefficients[i];
            }
            return coefficients;
        }

    }
}
