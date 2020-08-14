using Layout;
using System;
using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.EventHandler;
using LayoutFramework.CanvasItems;

namespace LayoutFramework.Layouts
{
    public class ColoredLayout : BaseLayout
    {
        public Color color = new Color(255, 255, 255, 255);

        private Action onClick;

        public ColoredLayout()
        {            
        }

        public void setOnClickListener(Action callback)
        {
            onClick = callback;
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem = base.calculateMeasuredLayout(perantBounds);
            calculatedItem.type = "ColoredLayout";
            calculatedItem.drawable = this;
            calculatedItem.reactiveView = this;
            return calculatedItem;
        }

        public override void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect rect)
        {
            if (color.a != 0)
            {
                ICanvasRectItem rectangle = new CanvasItemFactory().createCanvasRectItem();
                rectangle.setColor(color);
                rectangle.setSize(rect.Width, rect.Height);

                drawCanvas.drawToCanvas(rectangle, rect.Left, rect.Top);
            }
        }
    }
}
