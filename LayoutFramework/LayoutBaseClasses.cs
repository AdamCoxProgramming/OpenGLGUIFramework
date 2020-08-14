using System.Collections.Generic;
using System.Windows;
using static Layout.LayoutImplementations.BaseLayout;
using static Layout.LayoutRenderer;
using static Layout.EventHandler;

namespace Layout
{
    public class LayoutBaseClasses
    {

        public interface ILayoutItem
        {
            MeasuredLayout getMeasuredLayout(Bounds perantsRect);
        }

        public interface IDrawable
        {
            void draw(DrawCanvas drawCanvas, Rect boundsRect);
        }

        public class MeasuredLayout : IDrawable
        {
            Bounds bounds;
            List<MeasuredLayout> calculatedChildren = new List<MeasuredLayout>();
            public string type;
            public IDrawable drawable;
            public ReactiveView reactiveView;

            public void setBounds(Bounds bounds)
            {
                this.bounds = bounds;
            }

            public Bounds getBounds()
            {
                return bounds;
            }            

            public void addCalculatedChild(MeasuredLayout calculatedLayoutItem)
            {
                calculatedChildren.Add(calculatedLayoutItem);
            }

            public void clearChildren()
            {
                calculatedChildren.Clear();
            }

            public List<MeasuredLayout> getCalculatedChildren()
            {
                return calculatedChildren;
            }

            void IDrawable.draw(DrawCanvas drawCanvas,Rect boundsRect)
            {
                drawable.draw(drawCanvas, boundsRect);
            }
        }
    }
}
