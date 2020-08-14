using LayoutFramework.CanvasItems;
using System;
using System.Windows;
using static Layout.LayoutRenderer;

namespace LayoutFramework.Layouts.EditText
{
    public class CanvasScroller
    {
        private double scrollXInternal = 0;
        public double scrollX
        {
            get
            {
                if (contentsSize.Width < drawRegion.Width) return 0;
                double one = Math.Min(scrollXInternal, 0);
                double two = Math.Max(one, -(Math.Abs(contentsSize.Width - drawRegion.Width)));
                return two;
            }
            set
            {
                scrollXInternal = value;
            }
        }
        private double scrollYInternal = 0;
        public double scrollY
        {
            get
            {
                if (contentsSize.Height < drawRegion.Height) return 0;
                double one = Math.Min(scrollYInternal, 0);
                double two = Math.Max(one, -(contentsSize.Height - drawRegion.Height));
                return two;
            }
            set
            {
                scrollYInternal = value;
            }
        }

        public bool hideBars = false;

        private int scrollerBrightness = 60;

        private float scrollerWidth = 10;
        private int scrollerHeight = 70;
        private float scrollerPadding = 2;

        private bool draggingScrollY = false;
        private bool draggingScrollX = false;

        private Point prevScrollPos = new Point();
        private Rect drawRegion;
        private Rect contentsSize;

        public void drawScroller(DrawCanvas canvas, Rect drawRegion, Rect contentsSize)
        {
            this.drawRegion = drawRegion;
            this.contentsSize = contentsSize;

            if (hideBars) return;

            if (yScrollEnabled())
            {

                ICanvasRectItem rectangle = new CanvasItemFactory().createCanvasRectItem();
                rectangle.setColor(new Layout.Color(scrollerBrightness, scrollerBrightness, scrollerBrightness));
                rectangle.setSize(scrollerWidth, scrollerHeight);
                canvas.drawToCanvas(rectangle, drawRegion.Right - scrollerWidth - scrollerPadding, drawRegion.Top + scrollerPadding + getYScrollerYPos());
            }

            if (drawRegion.Width < contentsSize.Width)
            {

                ICanvasRectItem rectangle = new CanvasItemFactory().createCanvasRectItem();
                rectangle.setColor(new Layout.Color(scrollerBrightness, scrollerBrightness, scrollerBrightness));
                rectangle.setSize(scrollerHeight, scrollerWidth);
                canvas.drawToCanvas(rectangle, drawRegion.Left + scrollerPadding + getXScrollerXPos(), drawRegion.Bottom - scrollerWidth - scrollerPadding);
            }
        }

        private double getScrollAmountAsPercentageofHeight(Rect drawRegion, Rect contentsSize)
        {
            return (scrollY / (contentsSize.Height - drawRegion.Height)) * 100.0f;
        }

        public bool isScrolling()
        {
            return draggingScrollY || draggingScrollX;
        }

        public void stopScrolling()
        {
            draggingScrollY = false;
        }

        private bool yScrollEnabled()
        {
            return drawRegion.Height < contentsSize.Height;
        }

        private bool xScrollEnabled()
        {
            return drawRegion.Width < contentsSize.Width;
        }

        public bool handleClick(Point position)
        {
            if (isPosOverYScrollBar(position) && yScrollEnabled())
            {
                prevScrollPos.X = position.X;
                prevScrollPos.Y = position.Y;
                draggingScrollY = true;
                return true;
            }
            else if (isPosOverXScrollBar(position) && xScrollEnabled())
            {
                prevScrollPos.X = position.X;
                prevScrollPos.Y = position.Y;
                draggingScrollX = true;
                return true;
            }
            else return false;

        }

        private bool isPosOverYScrollBar(Point pos)
        {
            double left = drawRegion.Right - scrollerWidth - scrollerPadding;
            double top = getYScrollerYPos();
            int hitPadding = 3;
            bool xInRange = pos.X > left - hitPadding && pos.X < pos.X + scrollerWidth + hitPadding;
            bool yInRange = pos.Y > top - hitPadding && pos.Y < pos.Y + scrollerHeight + hitPadding;
            return xInRange && yInRange;
        }

        private bool isPosOverXScrollBar(Point pos)
        {
            double left = getXScrollerXPos();
            double top = drawRegion.Bottom - scrollerWidth - scrollerPadding;
            int hitPadding = 3;
            bool xInRange = pos.X > left - hitPadding && pos.X < left + scrollerHeight + hitPadding;
            bool yInRange = pos.Y > top - hitPadding && pos.Y < top + scrollerWidth + hitPadding;
            return xInRange && yInRange;
        }

        public bool handleMove(Point position)
        {
            if (draggingScrollY)
            {
                double percentageChange = ((position.Y - prevScrollPos.Y) / (drawRegion.Height - scrollerHeight - 2 * scrollerPadding));
                double totalAvailibleScrollArea = contentsSize.Height - drawRegion.Height;
                scrollY -= percentageChange * totalAvailibleScrollArea;
                prevScrollPos = position;
                return true;
            }
            else if (draggingScrollX)
            {
                double percentageChange = ((position.X - prevScrollPos.X) / (drawRegion.Width - scrollerHeight - 2 * scrollerPadding));
                double totalAvailibleScrollArea = contentsSize.Width - drawRegion.Width;
                scrollX -= percentageChange * totalAvailibleScrollArea;
                prevScrollPos = position;
                return true;
            }
            else return false;
        }

        private double getXScrollerXPos()
        {
            double currentScrollPercentAcrossBar = -scrollX / (contentsSize.Width - drawRegion.Width);
            return currentScrollPercentAcrossBar * (drawRegion.Width - scrollerHeight - 2 * scrollerPadding);
        }

        private double getYScrollerYPos()
        {
            double currentScrollPercentAcrossBar = -scrollY / (contentsSize.Height - drawRegion.Height);
            return currentScrollPercentAcrossBar * (drawRegion.Height - scrollerHeight - 2 * scrollerPadding);
        }


        public void handleUp(Point position)
        {
            draggingScrollY = false;
            draggingScrollX = false;
        }

    }
}
