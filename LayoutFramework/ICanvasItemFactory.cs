using LayoutFramework.CanvasItems;

namespace LayoutFramework
{
    public interface ICanvasItemFactory
    {
        ICanvasRectItem createCanvasRectItem();
        ICanvasTextItem createCanvasTextItem();
    }
}
