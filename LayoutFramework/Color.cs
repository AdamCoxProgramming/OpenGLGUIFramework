using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layout
{
    public struct Color
    {
        public int a, r, g, b;

        public Color( int r, int g, int b,int a)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 255;
        }
    }
}
