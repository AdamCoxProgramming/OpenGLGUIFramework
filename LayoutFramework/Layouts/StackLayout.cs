using Layout;
using System;
using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.EventHandler;
using LayoutFramework.CanvasItems;

namespace LayoutFramework.Layouts
{
    public class StackLayout : BaseLayout
    {
        public Color color = new Color(255, 255, 255, 255);

        private Action onClick;

        public StackLayout()
        {
            this.handleMouseEvent += mouseEventHandler;
        }

        public void setOnClickListener(Action callback)
        {
            onClick = callback;
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem = base.calculateMeasuredLayout(perantBounds);
            calculatedItem.type = "Stack";
            calculatedItem.drawable = this;
            calculatedItem.reactiveView = this;
            return calculatedItem;
        }

        private MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            if (motionEvent.eventType == MotionType.DOWN)
            {
                if (onClick != null) onClick();
                return new MouseHandleResult(motionEvent, HandledStatus.HANDLED,false);
            }
            else return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED,false);
        }

        public override void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect rect)
        {
        }
    }
}
