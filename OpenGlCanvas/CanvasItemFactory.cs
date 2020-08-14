using LayoutFramework;
using LayoutFramework.CanvasItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicWindow
{
    public class CanvasItemFactory : ICanvasItemFactory
    {
        public ICanvasRectItem createCanvasRectItem()
        {
            return new RectCanvasItem();
        }

        public ICanvasTextItem createCanvasTextItem()
        {
            return new TextCanvasItem();
        }
    }
}
