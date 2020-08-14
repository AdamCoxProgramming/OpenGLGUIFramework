using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using BasicWindow;
using System.Collections.Generic;
using OpenTK.Input;
using Layout;
using static LayoutFramework.Input.Callbacks;
using LayoutFramework.Input;
using System.Windows;
using System.Threading;

namespace BasicWindow
{
    public class OpenGLCanvas : GameWindow, ICanvas
    {
        private KeyEventDelegate keyPressedCallback;
        private KeyEventDelegate keyUpCallback;
        private ResizeDelegate resizeDelegate;
        private MouseEventDelegate mouseDownCallback;
        private MouseEventDelegate mouseMoveCallback;
        private MouseEventDelegate mouseUpCallback;

        private Object lockObj = new Object();

        private List<ICanvasItem> canvasItems = new List<ICanvasItem>();

        private Action updateCallback, loadCallback;

        private int clipX, clipY, clipWidth, clipHeight;
        private bool clippingEnabled = false;

        long lastDrawMilliseconds = DateTimeOffset.Now.ToUniversalTime().Millisecond;

        public OpenGLCanvas(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            MouseDown += Control_MouseDown;
            MouseMove += Control_MouseMove;
            MouseUp += Control_MouseUp;

            KeyDown += (object sender, KeyboardKeyEventArgs args) =>
            {
                if (keyPressedCallback != null)
                    keyPressedCallback((LayoutFramework.Keys.Key)args.Key);
            };

            KeyUp += (object sender, KeyboardKeyEventArgs args) =>
             {
                 if (keyUpCallback != null)
                     keyUpCallback((LayoutFramework.Keys.Key)args.Key);
             };

            Resize += (object sender, EventArgs args) =>
            {
                OnUpdateFrame(new FrameEventArgs());
                //if (resizeDelegate != null) resizeDelegate(Width, Height);
                //updateCallback();
            };

        }

        protected override void OnWindowInfoChanged(EventArgs e)
        {
            updateCallback();
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDownCallback != null)
                mouseDownCallback(e.X, e.Y);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseMoveCallback != null) mouseMoveCallback(e.X, e.Y);
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseUpCallback != null) mouseUpCallback(e.X, e.Y);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (loadCallback != null) loadCallback();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);// We clear the depth buffer in addition to the color buffer
            clippingEnabled = false;
            if (updateCallback != null) updateCallback();// The listener triggers the drawing to the canvas
            SwapBuffers();
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (IRenderable renderable in canvasItems)
            {
                renderable.destroy();
            }

            base.OnUnload(e);
        }


        void ICanvas.clearContents()
        {
            lock (lockObj)
            {
                canvasItems.Clear();
            }
        }

        void ICanvas.addChild(ICanvasItem item)
        {
            lock (lockObj)
            {
                canvasItems.Add(item);
            }
        }

        public void subscribeToFrameUpdates(Action callback)
        {
            updateCallback = callback;
        }

        public void subscribeToLoad(Action callback)
        {
            loadCallback = callback;
        }

        void ICanvas.subscribeToMouseClicks(MouseEventDelegate mouseClick)
        {
            mouseDownCallback = mouseClick;
        }

        void ICanvas.subscribeToMouseMove(MouseEventDelegate mouseMove)
        {
            mouseMoveCallback = mouseMove;
        }

        void ICanvas.subscribeToMouseUp(MouseEventDelegate mouseUp)
        {
            mouseUpCallback = mouseUp;
        }

        public void enableClipping(int x, int y, int width, int height)
        {
            this.clipX = x;
            this.clipY = y;
            this.clipWidth = width;
            this.clipHeight = height;
            clippingEnabled = true;
        }

        public void disableClipping()
        {
            clippingEnabled = false;
        }

        public void drawToCanvas(ICanvasItem canvasItem)
        {

            if (clippingEnabled)
            {
                GL.Enable(EnableCap.ScissorTest);
                //GL.Scissor(clipX, (Height - clipY - clipHeight), clipWidth, clipHeight);
                GL.Scissor(clipX, Height - clipY - clipHeight, clipWidth, clipHeight);
            }
            else
                GL.Disable(EnableCap.ScissorTest);


            Matrix4 _view = Matrix4.CreateTranslation(-(Width) / 2f, (Height) / 2f, -3.0f);
            Matrix4 _projection = Matrix4.CreateOrthographic(Width, Height, 0.1f, 100.0f);
            ((IGLCanvasItem)canvasItem).render(_view, _projection);

            GL.Disable(EnableCap.ScissorTest);
        }

        public void subscribeToResize(ResizeDelegate resizeDelegate)
        {
            this.resizeDelegate += resizeDelegate;
        }


        public void subscribeToKeyPress(KeyEventDelegate callback)
        {
            this.keyPressedCallback += callback;
        }

        public void subscribeToKeyRelease(KeyEventDelegate keyUpCallback)
        {
            this.keyUpCallback = keyUpCallback;
        }

        public Point getWindowDimensions()
        {
            return new Point(Width, Height);
        }

        public void update()
        {

        }
    }
}