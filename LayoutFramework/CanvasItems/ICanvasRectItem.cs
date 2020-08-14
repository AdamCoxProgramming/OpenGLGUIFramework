using Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutFramework.CanvasItems
{
    public interface ICanvasRectItem : ICanvasItem
    {
        void setColor(Color color);
        void setSize(double width, double height);
    }
}
