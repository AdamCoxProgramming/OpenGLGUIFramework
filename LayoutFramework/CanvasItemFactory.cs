using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LayoutFramework.CanvasItems;

namespace LayoutFramework
{
    public class CanvasItemFactory : ICanvasItemFactory
    {
        static ICanvasItemFactory currFactory;

        public static void setFactory(ICanvasItemFactory factory)
        {
            currFactory = factory;
        }

        private static ICanvasItemFactory getFactory()
        {
            if (currFactory == null) throw new Exception("Uninitiliazed");
            else return currFactory;
        }

        public ICanvasRectItem createCanvasRectItem()
        {
            ICanvasItemFactory factory = getFactory();
            return factory.createCanvasRectItem();
        }

        public ICanvasTextItem createCanvasTextItem()
        {
            ICanvasItemFactory factory = getFactory();
            return factory.createCanvasTextItem();
        }
    }
}
