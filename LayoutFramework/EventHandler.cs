using LayoutFramework;
using LayoutFramework.Input;
using System;
using System.Collections.Generic;
using System.Windows;
using static Layout.LayoutBaseClasses;

namespace Layout
{
    public class EventHandler
    {
        private ReactiveView latchedView = null;
        private static ReactiveView focusedView = null;

        private List<ReactiveView> previousMouseOverViews = new List<ReactiveView>();
        private List<ReactiveView> currentMouseOverViews = new List<ReactiveView>();


        public enum HandledStatus
        {
            HANDLED,
            NOT_HANDLED
        }

        public delegate void HandleKeyPressDelegate(KeyboardEvent keyboardEvent);

        public delegate MouseHandleResult HandleMouseDelegate(MouseEvent motionEvent);

        public abstract class ReactiveView
        {
            public HandleKeyPressDelegate handleKeyPress;

            public HandleMouseDelegate handleMouseEvent;

            public Action onFocus;

            public Action onLostFocus;


            public void requestFocus()
            {
                if (focusedView != null && focusedView.onLostFocus != null) focusedView.onLostFocus();
                focusedView = this;
                if (focusedView != null && focusedView.onFocus != null) focusedView.onFocus();
            }
        }

        public class MouseHandleResult
        {
            public MouseEvent motionEvent;
            public HandledStatus handled;
            public bool shouldLatch;

            public MouseHandleResult(MouseEvent motionEvent, HandledStatus handledStatus, bool shouldLatch)
            {
                this.motionEvent = motionEvent;
                this.handled = handledStatus;
                this.shouldLatch = shouldLatch;
            }
        }

        public void handleKeyboardInput(KeyboardEvent keyboardEvent)
        {
            if (focusedView != null && focusedView.handleKeyPress != null)
            {
                keyboardEvent.charValue = new GetCharForPressedKey().getCharForKey(keyboardEvent.keyPressed);
                focusedView.handleKeyPress(keyboardEvent);
            }
        }

        public void handleMouseEvent(MeasuredLayout calculatedTree, MouseEvent motionEvent)
        {
            currentMouseOverViews = new List<ReactiveView>();

            bool latchedLayoutHandledTheEvent = handleLatchedLayout(calculatedTree, motionEvent); // let the latched layout have first try
            //if (!latchedLayoutHandledTheEvent)
            handleItem(calculatedTree, motionEvent);

            notifyMouseOverListenterItems();
            previousMouseOverViews = currentMouseOverViews;
        }

        private void notifyMouseOverListenterItems()
        {
            MouseEvent mouseOverMotionEvent = new MouseEvent();
            mouseOverMotionEvent.eventType = MotionType.MOUSE_OVER; // does not currently implement pos

            MouseEvent mouseLeaveMotionEvent = new MouseEvent();
            mouseLeaveMotionEvent.eventType = MotionType.MOUSE_LEAVE; // does not currently implement pos

            foreach (ReactiveView reactiveView in currentMouseOverViews)
            {
                if (!previousMouseOverViews.Contains(reactiveView))
                    if (reactiveView.handleMouseEvent != null) reactiveView.handleMouseEvent(mouseOverMotionEvent);
            }

            foreach (ReactiveView reactiveView in previousMouseOverViews)
            {
                if (!currentMouseOverViews.Contains(reactiveView))
                    if (reactiveView.handleMouseEvent != null) reactiveView.handleMouseEvent(mouseLeaveMotionEvent);
            }
        }

        private bool handleItem(MeasuredLayout item, MouseEvent motionEvent)
        {

            if (isPointInBounds(motionEvent.coordinates, item.getBounds().rect))// Give a chance to handle and alter the motion event before passing on to children                
            {
                if (item.reactiveView != null && item.reactiveView != latchedView)
                {
                    currentMouseOverViews.Add(item.reactiveView);
                    if (item.reactiveView.handleMouseEvent != null)
                    {
                        MouseHandleResult motionResult = item.reactiveView.handleMouseEvent(motionEvent);

                        if (motionResult.shouldLatch == true)
                        {
                            latchedView = item.reactiveView;
                        }
                        if (motionResult.shouldLatch == false)
                        {
                            if (latchedView == item.reactiveView) latchedView = null;
                        }

                        if (motionResult.handled == HandledStatus.HANDLED)
                            return true;
                    }
                }

            }
            else return false;

            MouseEvent motionEventClone = motionEvent.clone();

            motionEventClone.coordinates.X -= item.getBounds().rect.Left;
            motionEventClone.coordinates.Y -= item.getBounds().rect.Top;

            foreach (MeasuredLayout childItem in item.getCalculatedChildren())
            {
                bool thisHandledIt = handleItem(childItem, motionEventClone);
                if (thisHandledIt == true)
                    return true;
            }

            return false;
        }

        private bool handleLatchedLayout(MeasuredLayout item, MouseEvent motionEvent)
        {
            if (item.reactiveView == latchedView)
            {
                MouseHandleResult res = item.reactiveView.handleMouseEvent(motionEvent);
                return res.handled == HandledStatus.HANDLED;
            }

            MouseEvent motionEventClone = motionEvent.clone();

            motionEventClone.coordinates.X -= item.getBounds().rect.Left;
            motionEventClone.coordinates.Y -= item.getBounds().rect.Top;

            bool childHandledIt = false;

            foreach (MeasuredLayout childItem in item.getCalculatedChildren())
            {
                if (handleLatchedLayout(childItem, motionEventClone)) childHandledIt = true;
            }
            return childHandledIt;
        }

        private bool isPointInBounds(Point point, Rect bounds)
        {
            return bounds.Contains(point);
        }

        public class KeyboardEvent
        {
            public Keys.Key keyPressed;
            public char charValue;
        }

        public class MouseEvent
        {
            public MotionType eventType;
            public Point coordinates;

            public MouseEvent clone()
            {
                MouseEvent motionEventClone = new MouseEvent();
                motionEventClone.eventType = eventType;
                motionEventClone.coordinates = coordinates;
                return motionEventClone;
            }
        }

        public enum MotionType
        {
            DOWN,
            MOVE,
            MOUSE_OVER,
            MOUSE_LEAVE,
            UP
        }

    }
}
