using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer.viola_jones
{
    class Stage
    {
        List<Tree> trees;
        float threshold;
        public Stage(float threshold)
        {
            this.threshold = threshold;
            trees = new List<Tree>();
            //features = new LinkedList<Feature>();
        }

        public void addTree(Tree t)
        {
            trees.Add(t);
        }

        public bool pass(int[,] grayImage, int[,] squares, int i, int j, float scale)
        {
            float sum = 0;
            foreach (Tree t in trees)
            {

                //System.out.println("Returned value :"+t.getVal(grayImage, squares,i, j, scale));

                sum += t.getVal(grayImage, squares, i, j, scale);
            }
            //System.out.println(sum+" "+threshold);
            return sum > threshold;
        }
    }
}
