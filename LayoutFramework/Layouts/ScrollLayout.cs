using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.LayoutRenderer;
using static Layout.EventHandler;

namespace LayoutFramework.Layouts
{
    public class ScrollLayout : BaseLayout
    {
        private Point prevCoord;

        private Point offset;

        private bool scrolling = false;

        public bool scrollX = false;

        public ScrollLayout()
        {
            this.handleMouseEvent += mouseEventHandler;
        }

        public void AddItem(BaseLayout item)
        {
            addChild(item);
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem = base.calculateMeasuredLayout(perantBounds);
            calculatedItem.type = "ScrollLayout";
            calculatedItem.drawable = this;
            calculatedItem.reactiveView = this;

            if (childLayoutItems.Count > 0)
            {
                MeasuredLayout child = childLayoutItems[0].getMeasuredLayout(perantBounds);
                Bounds childBounds = child.getBounds();
                childBounds.rect.Offset(new Vector(offset.X, offset.Y));
                calculatedItem.addCalculatedChild(child);
            }

            return calculatedItem;
        }

        public MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            if (motionEvent.eventType == MotionType.DOWN) scrolling = true;
            if (motionEvent.eventType == MotionType.UP) scrolling = false;
            if (motionEvent.eventType == MotionType.DOWN)
            {
                scrolling = true;
                return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED,true);
            }

            if (motionEvent.eventType == MotionType.MOVE && scrolling)
            {
                offset += (motionEvent.coordinates - prevCoord);
                if (scrollX) offset.Y = 0;
                else offset.X = 0;
            }

            prevCoord = new Point(motionEvent.coordinates.X, motionEvent.coordinates.Y);

            return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED,true); ;
        }

        public override void draw(DrawCanvas drawCanvas,Rect drawRect)
        {
            drawCanvas.enableClipping((int)drawRect.Left, (int)drawRect.Top, (int)drawRect.Width,(int)drawRect.Height);
            
        }
    }
}
