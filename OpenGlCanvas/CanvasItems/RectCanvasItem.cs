using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicWindow.Resources;
using Layout;
using LayoutFramework.CanvasItems;
using OpenTK;

namespace BasicWindow
{
    public class RectCanvasItem : ICanvasItem , IGLCanvasItem, ICanvasRectItem
    {
        private double x, y, width, height;

        private static RectRenderer rectRenderer = new RectRenderer();

        private Color drawColor = new Color(255, 0, 0);

        void IGLCanvasItem.render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            rectRenderer.setRect(new System.Windows.Rect(x, y, Math.Max( this.width,0), Math.Max( this.height,0)), drawColor);
            rectRenderer.render(viewMatrix, projectionMatrix);
        }

        void ICanvasItem.setTopLeft(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void setColor(Color color)
        {
            this.drawColor = color;
        }

        public void setSize(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

    }
}
