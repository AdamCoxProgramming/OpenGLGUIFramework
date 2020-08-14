using Layout;
using LearnOpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicWindow
{
    public class CanvasCreator
    {
        public ICanvas createCanvas(int width, int height, string title)
        {
            return new OpenGLCanvas(width,height,title);
        }
    }
}
