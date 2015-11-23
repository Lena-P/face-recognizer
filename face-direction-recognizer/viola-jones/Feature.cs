using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace face_direction_recognizer.viola_jones
{
    class Feature
    {
        Rectangle[] rects;
        int nb_rects;
        public float threshold;
        public float left_val;
        public float right_val;
        public Point size;
        public int left_node;
        public int right_node;
        public bool has_left_val;
        public bool has_right_val;

        public Feature(float threshold, float left_val, int left_node, bool has_left_val,
                float right_val, int right_node, bool has_right_val, Point size)
        {
            nb_rects = 0;
            rects = new Rectangle[3];
            this.threshold = threshold;
            this.left_val = left_val;
            this.left_node = left_node;
            this.has_left_val = has_left_val;
            this.right_val = right_val;
            this.right_node = right_node;
            this.has_right_val = has_right_val;
            this.size = size;
        }

        public int getLeftOrRight(int[,] grayImage, int[,] squares, int i, int j, float scale)
        {
            int w = (int)(scale * size.X);
            int h = (int)(scale * size.Y);
            double inv_area = 1.0/ (w * h);
            //System.out.println("w2 : "+w2);
            int total_x = grayImage[i + w,j + h] + grayImage[i,j] - grayImage[i,j + h] - grayImage[i + w,j];
            int total_x2 = squares[i + w,j + h] + squares[i,j] - squares[i,j + h] - squares[i + w,j];
            double moy = total_x * inv_area;
            double vnorm = total_x2 * inv_area - moy * moy;
            vnorm = (vnorm > 1) ? Math.Sqrt(vnorm) : 1;

            int rect_sum = 0;
            for (int k = 0; k < nb_rects; k++)
            {
                Rectangle r = rects[k];
                int rx1 = i + (int)(scale * r.x1);
                int rx2 = i + (int)(scale * (r.x1 + r.y1));
                int ry1 = j + (int)(scale * r.x2);
                int ry2 = j + (int)(scale * (r.x2 + r.y2));
                //System.out.println((rx2-rx1)*(ry2-ry1)+" "+r.weight);
                rect_sum += (int)((grayImage[rx2,ry2] - grayImage[rx1,ry2] - grayImage[rx2,ry1] + grayImage[rx1,ry1]) * r.weight);
            }
            //System.out.println(rect_sum);
            double rect_sum2 = rect_sum * inv_area;

            //System.out.println(rect_sum2+" "+threshold*vnorm);	
            return (rect_sum2 < threshold * vnorm) ? Tree.LEFT : Tree.RIGHT;

        }

        public void add(Rectangle r)
        {
            rects[nb_rects++] = r;
        }
    }
}
