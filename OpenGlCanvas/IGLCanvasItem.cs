using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicWindow
{
    interface IGLCanvasItem
    {
        void render(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }
}
