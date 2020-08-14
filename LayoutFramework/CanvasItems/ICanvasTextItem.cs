using Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LayoutFramework.CanvasItems
{
    public interface ICanvasTextItem : ICanvasItem
    {
        Rect measureText(string text);
        void setText(string text);
        void setTextSize(double fontSize);
        void setTextColor(Color color);
    }
}
