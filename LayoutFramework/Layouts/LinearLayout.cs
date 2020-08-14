using System.Windows;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.LayoutRenderer;
using static Layout.EventHandler;
using System;

namespace LayoutFramework.Layouts
{
    public class LinearLayout : BaseLayout
    {
        private bool vertical_orientation = true;
        public bool invertDirection = false;

        public enum Direction
        {
            VERTICAL,
            HORIZONTAL
        };

        public LinearLayout()
        {
            this.handleMouseEvent += mouseEventHandler;
        }

        public void setDirection(Direction direction)
        {
            if (direction == Direction.VERTICAL) vertical_orientation = true;
            else vertical_orientation = false;
        }

        public void addItem(BaseLayout item)
        {
            addChild(item);
        }

        public void clearItems()
        {
            clearChildren();
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            Bounds resultBounds = perantBounds.clone();

            MeasuredLayout calculatedRootItem = new MeasuredLayout();
            calculatedRootItem.type = "StackLayout";
            calculatedRootItem.drawable = this;
            calculatedRootItem.reactiveView = this;

            Bounds itemBounds = calculateBounds(resultBounds.rect, new Rect());

            if (sizeParams.Width == WRAP_CONTENTS) // This gives the child a chance to match to this items perants dimension if this item is wrapping
                itemBounds.rect.Width = perantBounds.rect.Width;
            if (sizeParams.Height == WRAP_CONTENTS)
                itemBounds.rect.Height = perantBounds.rect.Height;

            calculatedRootItem.setBounds(itemBounds);

            Point maxChildDimensions = getMaxFixedChildDimensions(calculatedRootItem.getBounds());

            double largestX = maxChildDimensions.X;
            double largestY = maxChildDimensions.Y;
            double yOffset = 0;
            double xOffset = 0;

            foreach (ILayoutItem layoutItem in childLayoutItems)
            {
                Bounds boundsOffsetFromStart = calculatedRootItem.getBounds().clone();

                if (!vertical_orientation && sizeParams.Height == WRAP_CONTENTS)
                    boundsOffsetFromStart.rect.Height = largestY;

                if (vertical_orientation && sizeParams.Width == WRAP_CONTENTS)
                    boundsOffsetFromStart.rect.Width = largestX;

                // The new bounds for each child must take into account the space that has allready been used UNLESS ...
                if (vertical_orientation)
                    boundsOffsetFromStart.rect = new Rect(new Point(0, yOffset), new Size(boundsOffsetFromStart.rect.Width, Math.Max(boundsOffsetFromStart.rect.Height - yOffset, 0)));
                else if (!vertical_orientation)
                    boundsOffsetFromStart.rect = new Rect(new Point(xOffset, 0), new Size(Math.Max(boundsOffsetFromStart.rect.Width - xOffset,0), boundsOffsetFromStart.rect.Height));

                // .. unless if the child uses a pecentage of the perants dimension, in which case pass the original width

                // Use the original perants width if the child is using a percentage of the perants width (in this case the space currently used is ignored)
                if (((BaseLayout)layoutItem).sizeParams.WidthPercent > 0)
                {
                    boundsOffsetFromStart.rect.Width = perantBounds.rect.Width;
                    boundsOffsetFromStart.rect.X = perantBounds.rect.X;
                }
                // Use the original perants height if the child is using a percentage of the perants height
                if (((BaseLayout)layoutItem).sizeParams.HeightPercent > 0)
                {
                    boundsOffsetFromStart.rect.Height = perantBounds.rect.Height;
                    boundsOffsetFromStart.rect.Y = perantBounds.rect.Y;
                }

                MeasuredLayout nextChild = layoutItem.getMeasuredLayout(boundsOffsetFromStart);
                nextChild.setBounds(offsetBounds(yOffset, xOffset, nextChild.getBounds()));

                calculatedRootItem.addCalculatedChild(nextChild);

                yOffset += nextChild.getBounds().rect.Height;
                xOffset += nextChild.getBounds().rect.Width;
            }


            // if the bounds were set to the perants (if the param was wrap) then change to the calcualted dimension
            if (vertical_orientation)
            {
                if (sizeParams.Width == WRAP_CONTENTS) calculatedRootItem.getBounds().rect.Width = largestX;
                if (sizeParams.Height == WRAP_CONTENTS) calculatedRootItem.getBounds().rect.Height = yOffset;
            }
            else
            {
                if (sizeParams.Width == WRAP_CONTENTS) calculatedRootItem.getBounds().rect.Width = xOffset;
                if (sizeParams.Height == WRAP_CONTENTS) calculatedRootItem.getBounds().rect.Height = largestY;
            }


            if (invertDirection)
                inverseBounds(calculatedRootItem, !vertical_orientation, vertical_orientation);

            return calculatedRootItem;
        }

        /* this function returns the largest dimensions of the children whose dimensions are not dependant on its perant */
        private Point getMaxFixedChildDimensions(Bounds perantDimensions)
        {
            double largestX = 0;
            double largestY = 0;

            foreach (ILayoutItem layoutItem in childLayoutItems)
            {
                MeasuredLayout nextChild = layoutItem.getMeasuredLayout(perantDimensions);

                if (!isHeightDependantOnParent(((BaseLayout)layoutItem).sizeParams))//if the height is dependant on the perants height, we dont know it yet as its calculated here
                    if (nextChild.getBounds().rect.Height > largestY)
                        largestY = nextChild.getBounds().rect.Height;

                if (!isWidthDependantOnParent(((BaseLayout)layoutItem).sizeParams)) //if the width is dependant on the perants width, we dont know it yet as its calculated here
                    if (nextChild.getBounds().rect.Width > largestX)
                        largestX = nextChild.getBounds().rect.Width;

            }
            return new Point(largestX, largestY);
        }

        bool isWidthDependantOnParent(SizeParams parameters)
        {
            return parameters.Width == MATCH_PARENT || parameters.Width == FILL || parameters.WidthPercent > 0;
        }

        bool isHeightDependantOnParent(SizeParams parameters)
        {
            return parameters.Height == MATCH_PARENT || parameters.Height == FILL || parameters.HeightPercent > 0;
        }

        private void inverseBounds(MeasuredLayout item, bool xAxis, bool yAxis)
        {
            foreach (MeasuredLayout childItem in item.getCalculatedChildren())
            {
                if (xAxis)
                    childItem.getBounds().rect.Offset(item.getBounds().rect.Width - childItem.getBounds().rect.Right - childItem.getBounds().rect.Left, 0);
                if (yAxis)
                    childItem.getBounds().rect.Offset(0, item.getBounds().rect.Height - childItem.getBounds().rect.Top - childItem.getBounds().rect.Bottom);
            }
        }

        public MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED, false);
        }

        private Bounds offsetBounds(double yOffset, double xOffset, Bounds initialBounds)
        {
            if (vertical_orientation)
                initialBounds.rect.Location = new Point(initialBounds.rect.Left, yOffset);
            else
                initialBounds.rect.Location = new Point(xOffset, initialBounds.rect.Top);
            return initialBounds;
        }

        public override void draw(DrawCanvas drawCanvas, Rect boundsRect)
        {

        }
    }
}
