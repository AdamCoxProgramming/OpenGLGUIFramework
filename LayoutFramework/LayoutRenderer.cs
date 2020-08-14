using System.Windows;
using static Layout.LayoutBaseClasses;

namespace Layout
{
    public class LayoutRenderer
    {
        private DrawCanvas canvas;

        public LayoutRenderer(ICanvas canvas)
        {
            this.canvas = new DrawCanvas(canvas);
        }

        public void drawTree(MeasuredLayout item)
        {
            canvas.clear();
            canvas.resetTopLeft();
            ((IDrawable)item).draw(canvas, item.getBounds().rect);
            drawTree(item, canvas);
        }

        private void drawTree(MeasuredLayout item, DrawCanvas canvas)
        {
            foreach (MeasuredLayout child in item.getCalculatedChildren())
            {
                DrawCanvas canvasClone = canvas.Clone();
                canvasClone.setTopLeft(item.getBounds().rect.Left, item.getBounds().rect.Top);

                child.drawable.draw(canvasClone, child.getBounds().rect);
                drawTree(child, canvasClone);
            }
        }

        public class DrawCanvas
        {
            protected ICanvas canvas;

            protected double startX = 0, startY = 0;

            private Rect clipingBounds;
            private bool clippingEnbaled = false;

            public DrawCanvas(ICanvas canvas)
            {
                this.canvas = canvas;
            }

            public void drawToCanvas(ICanvasItem uIElement, double x, double y)
            {
                uIElement.setTopLeft(startX + x, startY + y);
                if (clippingEnbaled)
                {
                    canvas.enableClipping((int)(clipingBounds.X), (int)(clipingBounds.Y), (int)clipingBounds.Width, (int)clipingBounds.Height);
                }
                else
                    canvas.disableClipping();

                canvas.drawToCanvas(uIElement);
            }

            public void clear()
            {
                canvas.clearContents();
            }

            public void resetTopLeft()
            {
                startX = 0;
                startY = 0;
            }

            public void setTopLeft(double x, double y)
            {
                this.startX += x;
                this.startY += y;
            }

            public void enableClipping(int x, int y, int width, int height)
            {
                if (clippingEnbaled)
                    clipingBounds.Intersect(new Rect(x + startX, y + startY, width, height));//.Intersect(clipingBounds);
                else
                    clipingBounds = new Rect(x + startX, y + startY, width, height);

                clippingEnbaled = true;
            }

            public void disableClipping()
            {
                clippingEnbaled = false;
            }

            public DrawCanvas Clone()
            {
                DrawCanvas newCanvas = new DrawCanvas(canvas);

                newCanvas.startX = startX;
                newCanvas.startY = startY;
                newCanvas.clipingBounds = clipingBounds;
                newCanvas.clippingEnbaled = clippingEnbaled;

                return newCanvas;
            }
        }
    }
}
