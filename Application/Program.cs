using App.JsonCompare;
using BasicWindow;
using Layout;
using LayoutFramework;
using System;
using System.Windows;
using static Layout.EventHandler;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.LayoutImplementations.BaseLayout;

namespace App
{
    class Program
    {        
        static void Main(string[] args)
        {
            new MainRunnable().run();
        }

        class MainRunnable
        {
            // It would be good to push some of this into a new "OpenGLApplication" layer leaving the this detail out of the application layer

            private int width = 1000;
            private int height = 800;

            private ICanvas canvas = null;

            private object lockObj = new Object();

            private BaseLayout rootLayout;
            private MeasuredLayout calculatedTree;
            private Layout.EventHandler reactiveLayout = new Layout.EventHandler();
            private LayoutRenderer layoutRenderer;

            public void run()
            {
                LayoutFramework.CanvasItemFactory.setFactory(new BasicWindow.CanvasItemFactory());

                canvas = new CanvasCreator().createCanvas(width, height, "{DIFF}");
                
                layoutRenderer = new LayoutRenderer(canvas);

                rootLayout = new LayoutLoader().loadLayout();

                canvas.subscribeToKeyPress((LayoutFramework.Keys.Key keyPressed) => {
                    lock (lockObj)
                    {
                        KeyboardEvent keyEvent = new KeyboardEvent();
                        keyEvent.keyPressed = keyPressed;
                        Keyboard.notifyKeyPressed(keyPressed);
                        if (calculatedTree != null) reactiveLayout.handleKeyboardInput(keyEvent);
                    }
                });

                canvas.subscribeToKeyRelease((LayoutFramework.Keys.Key keyPressed) => {
                    lock (lockObj)
                    {                        
                        Keyboard.notifyKeyUp(keyPressed);
                    }
                });               

                canvas.subscribeToMouseClicks((int x, int y) =>
                {
                    lock (lockObj)
                    {
                        MouseEvent motionEvent = new MouseEvent();
                        motionEvent.eventType = MotionType.DOWN;
                        motionEvent.coordinates = new Point(x, y);
                        if (calculatedTree != null) reactiveLayout.handleMouseEvent(calculatedTree, motionEvent);
                    }
                });

                canvas.subscribeToMouseUp((int x, int y) =>
                {
                    lock (this)
                    {
                        MouseEvent motionEvent = new MouseEvent();
                        motionEvent.eventType = MotionType.UP;
                        motionEvent.coordinates = new Point(x, y);
                        if (calculatedTree != null) reactiveLayout.handleMouseEvent(calculatedTree, motionEvent);
                    }
                });

                canvas.subscribeToMouseMove((int x, int y) =>
                {
                    lock (this)
                    {
                        MouseEvent motionEvent = new MouseEvent();
                        motionEvent.eventType = MotionType.MOVE;
                        motionEvent.coordinates = new Point(x, y);
                        if (calculatedTree != null) reactiveLayout.handleMouseEvent(calculatedTree, motionEvent);
                    }
                });

                canvas.subscribeToFrameUpdates(() =>
                {
                    lock (this)
                    {
                        TaskRunner.RunTasks();
                        MeasuredLayout result = rootLayout.getMeasuredLayout(getWindowBounds());
                        layoutRenderer.drawTree(result);
                        calculatedTree = result;
                    }
                });

                canvas.Run();
            }

            private Bounds getWindowBounds()
            {
                Point windowDimensions = canvas.getWindowDimensions();

                Bounds rootBounds = new Bounds();
                Rect rootRect = new Rect();
                rootRect.Width = windowDimensions.X;
                rootRect.Height = windowDimensions.Y;
                rootBounds.rect = rootRect;
                return rootBounds;
            }
        }
    }
}
