using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer.viola_jones
{
    class Tree
    {
        public static int LEFT = 0;
        public static int RIGHT = 1;

        List<Feature> features;

        public Tree()
        {
            features = new List<Feature>();
        }
        public void addFeature(Feature f)
        {
            features.Add(f);
        }

        public float getVal(int[,] grayImage, int[,] squares, int i, int j, float scale)
        {
            Feature cur_node = features[0];
            while (true)
            {
                int where = cur_node.getLeftOrRight(grayImage, squares, i, j, scale);
                if (where == LEFT)
                {
                    if (cur_node.has_left_val)
                    {
                        //System.out.println("LEFT");
                        return cur_node.left_val;
                    }
                    else
                    {
                        //System.out.println("REDIRECTION !");
                        //System.exit(0);
                        cur_node = features[cur_node.left_node];
                    }
                }
                else
                {
                    if (cur_node.has_right_val)
                    {

                        //System.out.println("RIGHT");
                        return cur_node.right_val;
                    }
                    else
                    {
                        //System.out.println("REDIRECTION !");
                        //System.exit(0);
                        cur_node = features[cur_node.right_node];
                    }
                }
            }
        }
    }
}
