using System;
using System.Threading;
using System.Windows;
using static LayoutFramework.Input.Callbacks;

namespace Layout
{
    public interface ICanvas
    {
        void clearContents();
        void drawToCanvas(ICanvasItem canvasItem);
        void subscribeToFrameUpdates(Action callback);
        void subscribeToLoad(Action callback);
        void addChild(ICanvasItem item);
        void subscribeToMouseClicks(MouseEventDelegate mouseClick);
        Point getWindowDimensions();
        void subscribeToMouseMove(MouseEventDelegate mouseMove);
        void subscribeToMouseUp(MouseEventDelegate mouseMove);
        void subscribeToKeyPress(KeyEventDelegate keyPressCallback);
        void subscribeToKeyRelease(KeyEventDelegate keyUpCallback);
        void enableClipping(int x, int y, int width, int height);
        void disableClipping();
        void Run();
        void update();
    }
}
