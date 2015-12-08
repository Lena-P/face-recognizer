using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace face_direction_recognizer
{
    class PupilSearcher
    {
        int matrixSize = 15;
        int half = 7;
        double[,] radialMask;
        double[,] gradMask;
        double[,,] cube;
        int maxRad;
        int minRad;

        public PupilSearcher()
        {
            InitMatrixes();
        }

        void InitMatrixes()
        {
            radialMask = new double[matrixSize, matrixSize];
            gradMask = new double[matrixSize, matrixSize];
            double min = matrixSize;
            double max = 0;
            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    radialMask[i, j] = (int)Math.Sqrt((i - half) * (i - half) + (j - half) * (j - half));
                    gradMask[i, j] = Math.Atan2(i-half, j-half);
                    if (radialMask[i, j] > max)
                    {
                        max = radialMask[i, j];
                    }
                    else
                    {
                        if (radialMask[i,j] < min)
                        {
                            min = radialMask[i, j];
                        }
                    }
                }
            }
            maxRad = (int)max;
            minRad = (int)min;

        }

        public Rectangle FindPupil(FastBitmap bitmap, Rectangle eye, double[,] angulars)
        {
            cube = new double[eye.Width, eye.Height, maxRad - minRad];
            double maxCubeval = 0;
            int maxi = 0, maxj = 0, maxr = 0;
            Parallel.For(0, eye.Width - (int)(0.5 * eye.Width), i =>
            //for (int i = 0; i < eye.Width; i++)
            {
                  for (int j = 0; j < eye.Height - 0.5 * eye.Height; j++)
                  {
                      for (int r = 0; r < maxRad - minRad; r++)
                      {
                          cube[i, j, r] = CountCubeValue(i, j, bitmap, eye, angulars);
                          if (maxCubeval <= cube[i, j, r])
                          {
                              maxi = i;
                              maxj = j;
                              maxr = r;
                              maxCubeval = cube[i, j, r];
                          }
                      }
                  }
              });

            Rectangle result = new Rectangle(eye.X + maxi, eye.Y + maxj, minRad + maxr, minRad + maxr);
            return result;
        }

        double CountCubeValue(int i, int j, FastBitmap bmp, Rectangle eye, double[,] angulars)
        {
            double result = 0;
            for (int k = 0; k < matrixSize; k++)
            {
                for (int l = 0; l < matrixSize; l++)
                {
                    int eyeX = eye.X + i - half + k;
                    int eyeY = eye.Y + j - half + l;
                    double difAng = angulars[eyeX, eyeY] - gradMask[k,l];
                    result += bmp[eyeX, eyeY] * Math.Cos(difAng) * radialMask[k,l];
                }
            }
            return result;
        }
    }
}
