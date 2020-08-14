using System.Windows;
using Layout;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.EventHandler;
using LayoutFramework.CanvasItems;

namespace LayoutFramework.Layouts
{
    public class CustomPlacementLayout : BaseLayout
    {
        public Color color = new Color(0,0,0,0);

        public CustomPlacementLayout()
        {
            this.handleMouseEvent += mouseEventHandler;
        }

        public void AddItem(BaseLayout item)
        {
            addChild(item);
        }

        public override void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect boundsRect)
        {
            if (color.a != 0)
            {
                ICanvasRectItem rectangle = new CanvasItemFactory().createCanvasRectItem();
                rectangle.setColor(color);
                rectangle.setSize(boundsRect.Width, boundsRect.Height);

                drawCanvas.drawToCanvas(rectangle, boundsRect.Left, boundsRect.Top);
            }
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedRootItem = base.calculateMeasuredLayout(perantBounds);
            calculatedRootItem.type = "CustomPlacementLayout";
            calculatedRootItem.drawable = this;
            calculatedRootItem.reactiveView = this;
            
            Bounds resultBounds = calculatedRootItem.getBounds().clone();

            double largestX = 0;
            double largestY = 0;

            foreach (ILayoutItem layoutItem in childLayoutItems)
            {
                MeasuredLayout nextChild = layoutItem.getMeasuredLayout(resultBounds);
                nextChild.reactiveView = (ReactiveView)layoutItem;

                double width = nextChild.getBounds().rect.Width;
                double height = nextChild.getBounds().rect.Height;

                if (width > largestX) largestX = width;
                if (height > largestY) largestY = height;

                calculatedRootItem.addCalculatedChild(nextChild);
            }

            if (sizeParams.Width == WRAP_CONTENTS)
            {
                calculatedRootItem.getBounds().rect.Width = largestX;

                foreach (MeasuredLayout layoutItem in calculatedRootItem.getCalculatedChildren())
                    layoutItem.getBounds().rect.Location = new Point(layoutItem.getBounds().rect.Location.X + largestX / 2, layoutItem.getBounds().rect.Location.Y);
            }
            if (sizeParams.Width == WRAP_CONTENTS)
            {
                calculatedRootItem.getBounds().rect.Height = largestY;

                foreach (MeasuredLayout layoutItem in calculatedRootItem.getCalculatedChildren())
                    layoutItem.getBounds().rect.Location = new Point(layoutItem.getBounds().rect.Location.X, layoutItem.getBounds().rect.Location.Y + largestY / 2);
            }
            
            return calculatedRootItem;
        }

        public MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED,false);
        }
    }

}
