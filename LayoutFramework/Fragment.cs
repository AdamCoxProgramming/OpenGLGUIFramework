using System.Windows;
using Layout;
using static Layout.EventHandler;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;

namespace LayoutFramework
{
    public abstract class Fragment : BaseLayout
    {
        private BaseLayout content  = null;

        public Fragment()
        {
            content = getContent();
            addChild(content);
            setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedRootItem = new MeasuredLayout();
            calculatedRootItem.type = "Fragment";
            calculatedRootItem.drawable = this;
            calculatedRootItem.reactiveView = this;
            calculatedRootItem.setBounds(perantBounds.clone());

            double largestX = 0;
            double largestY = 0;

            foreach (ILayoutItem layoutItem in childLayoutItems)
            {
                MeasuredLayout nextChild = layoutItem.getMeasuredLayout(perantBounds.clone());
                nextChild.reactiveView = (ReactiveView)layoutItem;

                nextChild.getBounds().rect.Location = new Point(0, 0);

                double width = nextChild.getBounds().rect.Width;
                double height = nextChild.getBounds().rect.Height;

                if (width > largestX) largestX = width;
                if (height > largestY) largestY = height;

                calculatedRootItem.addCalculatedChild(nextChild);
            }

            if (sizeParams.Width == WRAP_CONTENTS)
            {
                calculatedRootItem.getBounds().rect.Width = largestX;
            }
            if (sizeParams.Width == WRAP_CONTENTS)
            {
                calculatedRootItem.getBounds().rect.Height = largestY;
            }

            return calculatedRootItem;
        }

        public abstract BaseLayout getContent();

        public override void draw(LayoutRenderer.DrawCanvas drawCanvas, Rect boundsRect)
        {

        }
    }
}
