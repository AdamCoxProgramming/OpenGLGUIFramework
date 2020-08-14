using System.Collections.Generic;
using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.EventHandler;
using System;

namespace Layout
{
    public class LayoutImplementations
    {
        public abstract class BaseLayout : ReactiveView, ILayoutItem, IDrawable
        {
            public SizeParams sizeParams = new SizeParams();
            protected PositionParams positionParams = new PositionParams();

            protected List<ILayoutItem> childLayoutItems = new List<ILayoutItem>();

            public void addChild(ILayoutItem layoutItem)
            {
                childLayoutItems.Add(layoutItem);
            }

            public int getChildCount()
            {
                return childLayoutItems.Count;
            }

            public void removeChild(int index)
            {
                childLayoutItems.RemoveAt(index);
            }

            public List<ILayoutItem> getChildren()
            {
                return childLayoutItems;
            }

            public void removeChild(ILayoutItem item)
            {
                if (!childLayoutItems.Contains(item)) throw new Exception("Item not in list");
                else childLayoutItems.Remove(item);
            }

            protected void clearChildren()
            {
                childLayoutItems.Clear();
            }

            public void setSizeParams(SizeParams sizeParams)
            {
                this.sizeParams = sizeParams;
            }

            public void setPositionParams(PositionParams positionParams)
            {
                this.positionParams = positionParams;
            }

            protected Bounds calculateBounds(Rect perantsRect, Rect contentRect)
            {
                return calcualteChildRect(perantsRect, sizeParams, positionParams, contentRect);
            }

            public MeasuredLayout getMeasuredLayout(Bounds perantRect)
            {

                if (perantRect.rect.Width == 0 || perantRect.rect.Height == 0 || perantRect.rect.IsEmpty)
                {
                    MeasuredLayout calculatedRootItem = new MeasuredLayout();
                    calculatedRootItem.drawable = this;
                    calculatedRootItem.reactiveView = this;
                    calculatedRootItem.type = "Empty layout beacause the perant had no dimensions";
                    calculatedRootItem.setBounds(new Bounds() { rect = new Rect(perantRect.rect.Location, new Size(0, 0)) }); // dont try calculating the child if no size 
                    return calculatedRootItem;
                }
                else return calculateMeasuredLayout(perantRect);

            }

            protected virtual MeasuredLayout calculateMeasuredLayout(Bounds perantRect)
            {
                MeasuredLayout calculatedRootItem = new MeasuredLayout();
                calculatedRootItem.drawable = this;
                calculatedRootItem.reactiveView = this;
                calculatedRootItem.type = "base measure";
                calculatedRootItem.setBounds(calculateBounds(perantRect.rect, new Rect()));
                return calculatedRootItem;
            }

            public static int MATCH_PARENT = -1;
            public static int FILL = -2;
            public static int CENTER = -3;
            public static int WRAP_CONTENTS = -4;

            private static Bounds calcualteChildRect(Rect parentRect, SizeParams sizeParams, PositionParams positionParams, Rect contentRect)
            {
                Bounds bounds = new Bounds();

                int childWidth = 0;

                if (sizeParams.Width == WRAP_CONTENTS) childWidth = (int)contentRect.Width;
                else if (sizeParams.WidthPercent != FILL && sizeParams.WidthPercent != MATCH_PARENT)
                    childWidth = (int)(parentRect.Width * sizeParams.WidthPercent / 100.0f);
                else if (sizeParams.Width == MATCH_PARENT)
                    childWidth = (int)parentRect.Width;
                else if (sizeParams.Width >= 0) childWidth = (int)sizeParams.Width;
                else childWidth = (int)parentRect.Width;

                int childHeight = 0;
                if (sizeParams.Height == WRAP_CONTENTS) childHeight = (int)contentRect.Height;
                else if (sizeParams.HeightPercent != FILL && sizeParams.HeightPercent != MATCH_PARENT) childHeight = (int)(parentRect.Height * sizeParams.HeightPercent / 100.0f);
                else if (sizeParams.Height == MATCH_PARENT) childHeight = (int)parentRect.Height;
                else if (sizeParams.Height >= 0) childHeight = (int)sizeParams.Height;
                else childHeight = (int)parentRect.Height;

                int childXPos;
                if (positionParams.xPos == CENTER)
                    childXPos = (int)parentRect.Left + (int)((parentRect.Width / 2.0f) - childWidth / 2);
                else childXPos = (int)parentRect.Left + (int)positionParams.xPos;

                int childYPos;
                if (positionParams.yPos == CENTER)
                    childYPos = (int)parentRect.Top + (int)((parentRect.Height / 2.0f) - childHeight / 2);
                else childYPos = (int)parentRect.Top + (int)positionParams.yPos;

                bounds.rect = new Rect(childXPos, childYPos, childWidth, childHeight);

                return bounds;
            }

            public abstract void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect boundsRect);

            public class Bounds // This class should be removed, it no longer has any benifit!
            {
                public Rect rect = new Rect();

                public Bounds clone()
                {
                    Bounds result = new Bounds();
                    result.rect = new Rect(rect.TopLeft, rect.BottomRight);
                    result.rect.Width = rect.Width;
                    result.rect.Height = rect.Height;
                    return result;
                }

            }

            public class SizeParams
            {
                public SizeParams() { }

                public SizeParams(float width, float height)
                {
                    this.Width = width;
                    this.Height = height;
                }

                public float Width = FILL;
                public float WidthPercent = FILL;

                public float Height = FILL;
                public float HeightPercent = FILL;
            }

            public class PositionParams
            {

                public PositionParams() { }

                public PositionParams(float xPos, float yPos)
                {
                    this.xPos = xPos;
                    this.yPos = yPos;
                }

                public float xPos = 0;
                public float yPos = 0;
            }
        }
    }
}
