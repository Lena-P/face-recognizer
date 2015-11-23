using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer.viola_jones
{
    class Rectangle
    {
        public int x1, x2, y1, y2;
        public float weight;
        public Rectangle(int x1, int x2, int y1, int y2, float weight)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.weight = weight;
        }
        public static Rectangle fromString(String text)
        {
            String[] tab = text.Split(' ');
            int x1 = int.Parse(tab[0]);
            int x2 = int.Parse(tab[1]);
            int y1 = int.Parse(tab[2]);
            int y2 = int.Parse(tab[3]);
            float f = float.Parse(tab[4], CultureInfo.InvariantCulture);
            return new Rectangle(x1, x2, y1, y2, f);
        }
    }
}
