using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Room_Tempture_simulator
{
    struct Square
    {
        public int X;
        public int Y;
        public int width;
        public int height;
        public int index;
        public int type;
        public double tempture;
        public bool done;
        public Color color;
    }
}
