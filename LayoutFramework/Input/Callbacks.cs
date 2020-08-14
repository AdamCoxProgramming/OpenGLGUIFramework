using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutFramework.Input
{
    public class Callbacks
    {
        public delegate void MouseEventDelegate(int x, int y);
        public delegate void KeyEventDelegate(LayoutFramework.Keys.Key keyPressed);
        public delegate void ResizeDelegate(int widht, int height);
    }
}
