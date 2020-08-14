using Layout;
using System;
using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.EventHandler;
using LayoutFramework.CanvasItems;

namespace LayoutFramework.Layouts
{
    public class ButtonLayout : BaseLayout
    {
        public Color color = new Color(125, 125, 125);

        private Action onClick;
        private double EDGE_WIDTH = 2;

        private bool mouseOverButton = false;

        public Object metaData = null;

        private BaseLayout contents = null;
        private MeasuredLayout contentsMeasuredLayout = null;
        public int contentPaddingHorizontal = 25;
        public int contentPaddingVertical = 7;

        public ButtonLayout()
        {
            this.handleMouseEvent += mouseEventHandler;
        }

        public void setContents(BaseLayout contents)
        {
            this.contents = contents;
        }

        public void setOnClickListener(Action callback)
        {
            onClick = callback;
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem;
            if (contents == null) {
                calculatedItem = base.calculateMeasuredLayout(perantBounds);
                calculatedItem.type = "ColoredLayout";
                calculatedItem.drawable = this;
                calculatedItem.reactiveView = this;
            }
            else {
                calculatedItem = new MeasuredLayout();
                calculatedItem.drawable = this;
                calculatedItem.reactiveView = this;
                contentsMeasuredLayout = contents.getMeasuredLayout(perantBounds);
                Rect contentRect = contentsMeasuredLayout.getBounds().rect;
                contentRect.Width += 2 * contentPaddingHorizontal;
                contentRect.Height += 2 * contentPaddingVertical;
                calculatedItem.setBounds(calculateBounds(perantBounds.rect, contentRect));
            }
            return calculatedItem;
        }

        private MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            if (motionEvent.eventType == MotionType.MOUSE_OVER) mouseOverButton = true;
            if (motionEvent.eventType == MotionType.MOUSE_LEAVE) mouseOverButton = false;

            if (motionEvent.eventType == MotionType.DOWN)
            {
                requestFocus();
                if (onClick != null) onClick();
                return new MouseHandleResult(motionEvent, HandledStatus.HANDLED, false);
            }
            else
            {   
                return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED, false);
            }
        }

        public override void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect rect)
        {     
            ICanvasRectItem edgeRectangle = new CanvasItemFactory().createCanvasRectItem();
            Color edgeColor = new Color(color.r + 30, color.g + 30, color.b + 30);
            edgeRectangle.setColor(edgeColor);
            edgeRectangle.setSize(rect.Width, rect.Height);
            drawCanvas.drawToCanvas(edgeRectangle, rect.Left, rect.Top);

            ICanvasRectItem rectangle = new CanvasItemFactory().createCanvasRectItem();
            Color buttonColor;
            if (mouseOverButton) buttonColor = new Color((int)(color.r + 15) , (int)(color.g + 15), (int)(color.b + 15));
            else buttonColor = color;
            rectangle.setColor(buttonColor);
            rectangle.setSize(rect.Width - 2* EDGE_WIDTH , rect.Height - 2* EDGE_WIDTH);
            drawCanvas.drawToCanvas(rectangle, rect.Left + EDGE_WIDTH, rect.Top + EDGE_WIDTH);

            if(contentsMeasuredLayout != null)
                contentsMeasuredLayout.drawable.draw(drawCanvas, contentsMeasuredLayout.getBounds().rect);
        }
    }
}
