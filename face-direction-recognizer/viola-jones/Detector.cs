﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Globalization;

namespace face_direction_recognizer.viola_jones
{
    class Detector
    {
        /** The list of classifiers that the test image should pass to be considered as an image.*/
        List<Stage> stages;
        Point size;
        /** Detector constructor.
         * Builds, from a XML file, the corresponding Haar cascade.
         * @param filename The XML file (generated by OpenCV) describing the Haar cascade.
         */

        public Detector(String filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            stages = new List<Stage>();
            
            XmlNode racine = doc.FirstChild.FirstChild;
            string[] dementions = racine["size"].InnerText.Split(' ');
            size = new Point(int.Parse(dementions[0]), int.Parse(dementions[1]));
            XmlNodeList it = racine["stages"].ChildNodes;
            foreach(XmlNode stage in it)
            {
                float thres = float.Parse(stage["stage_threshold"].InnerText, CultureInfo.InvariantCulture);
                //System.out.println(thres);
                XmlNodeList it2 = stage["trees"].ChildNodes;
                Stage st = new Stage(thres);
                foreach (XmlNode tree in it2)
                {
                    Tree t = new Tree();
                    XmlNodeList it4 = tree.ChildNodes;
                    foreach (XmlNode feature in it4)
                    {
                        float thres2 = float.Parse(feature["threshold"].InnerText, CultureInfo.InvariantCulture);
                        int left_node = -1;
                        float left_val = 0;
                        bool has_left_val = false;
                        int right_node = -1;
                        float right_val = 0;
                        bool has_right_val = false;
                        XmlNode e = feature["left_val"];
                        if (e != null)
                        {
                            left_val = float.Parse(e.InnerText, CultureInfo.InvariantCulture);
                            has_left_val = true;
                        }
                        else
                        {
                            left_node = int.Parse(feature["left_node"].InnerText);
                            has_left_val = false;
                        }
                        e = feature["right_val"];
                        if (e != null)
                        {
                            right_val = float.Parse(e.InnerText, CultureInfo.InvariantCulture);
                            has_right_val = true;
                        }
                        else
                        {
                            right_node = int.Parse(feature["right_node"].InnerText);
                            has_right_val = false;
                        }
                        Feature f = new Feature(thres2, left_val, left_node, has_left_val, right_val, right_node, has_right_val, size);
                        XmlNodeList it3 = feature["feature"]["rects"].ChildNodes;
                        foreach (XmlNode node in it3)
                        {
                            string s = node.InnerText.Trim();
                            //System.out.println(s);
                            Rectangle r = Rectangle.fromString(s);
                            f.add(r);
                        }

                        t.addFeature(f);
                    }
                    st.addTree(t);
                    //System.out.println("Number of nodes in tree "+t.features.size());
                }
                //System.out.println("Number of trees : "+ st.trees.size());
                stages.Add(st);
            }
                
            //System.out.println(stages.size());
        }

        /** Returns the list of detected objects in an image applying the Viola-Jones algorithm.
         * 
         * The algorithm tests, from sliding windows on the image, of variable size, which regions should be considered as searched objects.
         * Please see Wikipedia for a description of the algorithm.
         * @param image bufferedimage input
         * @param baseScale The initial ratio between the window size and the Haar classifier size (default 2).
         * @param scale_inc The scale increment of the window size, at each step (default 1.25).
         * @param increment The shift of the window at each sub-step, in terms of percentage of the window size.
         * @return the list of rectangles containing searched objects, expressed in pixels.
         */
        public List<System.Drawing.Rectangle> getElements(FastBitmap image, float baseScale, float scale_inc, float increment, int min_neighbors, System.Drawing.Rectangle rect)
        {
            Integrator integrator = new Integrator(image);
            List<System.Drawing.Rectangle> ret = new List<System.Drawing.Rectangle>();
            int width = rect.Width;
            int height = rect.Height;
            float maxScale = (Math.Min((width + 0.0f) / size.X, (height + 0.0f) / size.X));
            int[,] integralImage = integrator.IntegralView;
            int[,] squares = integrator.SquareIntegralView;
            
            for (float scale = baseScale; scale < maxScale; scale *= scale_inc)
            {
                int step = (int)(scale * 24 * increment);
                int ultraSize = (int)(scale * 24);
                Parallel.For(rect.X, rect.X + width - ultraSize, i =>
               {
                   if (i % step == 0)
                   {
                       for (int j = rect.Y; j < rect.Y + height - ultraSize; j += step)
                       {
                           bool pass = true;
                           int k = 0;
                           foreach (Stage s in stages)
                           {

                               if (!s.pass(integralImage, squares, i, j, scale))
                               {
                                   pass = false;
                                   break;
                                    //System.out.println("Failed at Stage "+k);
                                }
                               k++;
                           }
                           if (pass)
                           {
                               ret.Add(new System.Drawing.Rectangle(i, j, ultraSize, ultraSize));
                                // return merge(ret, min_neighbors);
                            }
                       }
                   }
               });
            }
            return merge(ret, min_neighbors);
        }

        public List<System.Drawing.Rectangle> merge(List<System.Drawing.Rectangle> rects, int min_neighbors)
        {
            List<System.Drawing.Rectangle> retour = new List<System.Drawing.Rectangle>();
            int[] ret = new int[rects.Count];
            int nb_classes = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < i; j++)
                {
                    if (equals(rects[j], rects[i]))
                    {
                        found = true;
                        ret[i] = ret[j];
                    }
                }
                if (!found)
                {
                    ret[i] = nb_classes;
                    nb_classes++;
                }
            }
            //System.out.println(Arrays.toString(ret));
            int[] neighbors = new int[nb_classes];
            System.Drawing.Rectangle[] rect = new System.Drawing.Rectangle[nb_classes];
            for (int i = 0; i < nb_classes; i++)
            {
                neighbors[i] = 0;
                rect[i] = new System.Drawing.Rectangle(0, 0, 0, 0);
            }
            for (int i = 0; i < rects.Count; i++)
            {
                neighbors[ret[i]]++;
                rect[ret[i]].X += rects[i].X;
                rect[ret[i]].Y += rects[i].Y;
                rect[ret[i]].Height += rects[i].Height;
                rect[ret[i]].Width += rects[i].Width;
            }
            for (int i = 0; i < nb_classes; i++)
            {
                int n = neighbors[i];
                if (n >= min_neighbors)
                {
                    System.Drawing.Rectangle r = new System.Drawing.Rectangle(0, 0, 0, 0);
                    r.X = (rect[i].X * 2 + n) / (2 * n);
                    r.Y = (rect[i].Y * 2 + n) / (2 * n);
                    r.Width = (rect[i].Width * 2 + n) / (2 * n);
                    r.Height = (rect[i].Height * 2 + n) / (2 * n);
                    retour.Add(r);
                }
            }
            return retour;
        }

        public bool equals(System.Drawing.Rectangle r1, System.Drawing.Rectangle r2)
        {
            int distance = (int)(r1.Width * 0.2);

            /*return r2.x <= r1.x + distance &&
                   r2.x >= r1.x - distance &&
                   r2.y <= r1.y + distance &&
                   r2.y >= r1.y - distance &&
                   r2.width <= (int)( r1.width * 1.2 ) &&
                   (int)( r2.width * 1.2 ) >= r1.width;*/
            if (r2.X <= r1.X + distance &&
                   r2.X >= r1.X - distance &&
                   r2.Y <= r1.Y + distance &&
                   r2.Y >= r1.Y - distance &&
                   r2.Width <= (int)(r1.Width * 1.2) &&
                   (int)(r2.Width * 1.2) >= r1.Width)
                return true;
            return r1.X >= r2.X && r1.X + r1.Width <= r2.X + r2.Width && r1.Y >= r2.Y && r1.Y + r1.Height <= r2.Y + r2.Height;
        }
    }
}
