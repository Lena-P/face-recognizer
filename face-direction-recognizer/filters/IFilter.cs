using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace face_direction_recognizer
{
    interface IFilter
    {
        void DoFilter(FastBitmap bitmap);
    }
}
