using Layout;
using LayoutFramework.CanvasItems;
using LayoutFramework.Layouts.EditText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static Layout.EventHandler;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.LayoutRenderer;

namespace LayoutFramework.Layouts
{
    public class EditTextLayout : BaseLayout
    {
        public Color backgroundColor = new Color(255, 255, 255);
        public Color edgeColor
        {
            get
            {
                if (isViewFocused)
                    return edgeColorInternal;
                else return greyEdgeColor;
            }
            set
            {
                edgeColorInternal = value;
            }

        }
        private Color edgeColorInternal = new Color(29, 116, 174);

        private Color greyEdgeColor = new Color(170, 170, 170);

        public float TEXT_PADDING = 2.5f;
        public float BORDER_SIZE = 2f;
        public int textSize = 15;
        public bool singleLine = false;

        private float CURSOR_WIDTH = 2f;
        private string textHidden = "";
        private string displayText {
            get
            {
                return textHidden;
            }
            set
            {
                textHidden = value;
                if (textChangedCallback != null) textChangedCallback();
            }
        }
        private bool isViewFocused = false;

        private int tabSpaces = 4;

        private int selectionBeganIndex;
        private int cursorIndex = 0;
        private Rect previousBounds;

        private CanvasScroller scroller = new CanvasScroller();

        private bool makingSelection = false;

        private Rect charecterDimensions;

        private List<Point> locationsPerIndex = new List<Point>();
        private int maxXPos = 0;
        private int maxYPos = 0;

        private Action textChangedCallback;

        public EditTextLayout()
        {
            ICanvasTextItem textMeasurer = new CanvasItemFactory().createCanvasTextItem();
            charecterDimensions = textMeasurer.measureText("a");

            this.handleMouseEvent += mouseEventHandler;
            this.handleKeyPress += keyboardEventHandler;
            this.onFocus += () =>
            {
                isViewFocused = true;
            };
            this.onLostFocus += () =>
            {
                makingSelection = false;
                scroller.stopScrolling();
                isViewFocused = false;
            };
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem = base.calculateMeasuredLayout(perantBounds);
            calculatedItem.drawable = this;
            calculatedItem.reactiveView = this;
            calculatedItem.type = "Edit text";
            return calculatedItem;
        }

        private void keyboardEventHandler(KeyboardEvent keyboardEvent)
        {

            if (keyboardEvent.keyPressed == Keys.Key.ControlLeft || keyboardEvent.keyPressed == Keys.Key.ControlRight) return;
            if (keyboardEvent.keyPressed == Keys.Key.ShiftLeft || keyboardEvent.keyPressed == Keys.Key.ShiftRight) return;
            if (keyboardEvent.keyPressed == Keys.Key.Up || keyboardEvent.keyPressed == Keys.Key.Down) return;

            int selectionLength = Math.Abs(cursorIndex - selectionBeganIndex);

            switch (keyboardEvent.keyPressed)
            {
                case Keys.Key.Back:
                    if (this.displayText.Length == 0) break;
                    if (selectionLength != 0) removeSelectionText();
                    else
                    {
                        removeTextAtCursor(1, 0);
                        cursorIndex--;
                    }
                    break;
                case Keys.Key.Delete:
                    if (this.displayText.Length == 0) break;
                    if (cursorIndex < displayText.Length)
                    {
                        if (selectionLength != 0) removeSelectionText();
                        else removeTextAtCursor(1, 1);
                    }
                    else if (selectionLength != 0) removeSelectionText();
                    break;
                case Keys.Key.V:
                    if (Keyboard.isControlDown())
                    {
                        string textToPaste = LayoutFramework.Input.Clipboard.getText();
                        if (singleLine) textToPaste = textToPaste.Replace("\n", "").Replace("\r","");
                        addTextAtCursor(textToPaste);
                    }
                    else addTextAtCursor(keyboardEvent.charValue + "");
                    break;
                case Keys.Key.CapsLock:
                    break;
                case Keys.Key.Enter:
                    if (!singleLine) addTextAtCursor("\n");
                    break;
                case Keys.Key.Tab:
                    addTextAtCursor("\t");
                    break;
                case Keys.Key.Right:
                    cursorIndex = Math.Min(cursorIndex + 1, displayText.Length);
                    break;
                case Keys.Key.Left:
                    cursorIndex = Math.Max(cursorIndex - 1, 0);
                    break;
                default:
                    addTextAtCursor(keyboardEvent.charValue + "");
                    break;
            }
            selectionBeganIndex = cursorIndex;
            calcualteAllTextPositions();
            updateScrollOffsetToKeepCursorOnScreen();
        }

        public void susbcribeToTextChanges(Action callback)
        {
            textChangedCallback = callback;
        }

        private void addTextAtCursor(string text)
        {
            displayText = displayText.Insert(getCursorIndex(), text);
            cursorIndex += text.Length;
        }

        private void removeSelectionText()
        {
            int selectionStartIndex = Math.Min(selectionBeganIndex, cursorIndex);
            int selectionEndIndex = Math.Max(selectionBeganIndex, cursorIndex);
            displayText = displayText.Remove(selectionStartIndex, selectionEndIndex - selectionStartIndex);
            cursorIndex = selectionStartIndex;
        }

        private void removeTextAtCursor(int numOfCharsToRemove, int cursorOffset)
        {
            displayText = displayText.Remove(Math.Max(getCursorIndex() - 1 + cursorOffset, 0), numOfCharsToRemove);
        }

        private MouseHandleResult mouseEventHandler(MouseEvent motionEvent)
        {
            if (motionEvent.eventType == MotionType.DOWN)
            {
                this.requestFocus();
                bool scrollerHandledClick = false;
                if (!singleLine) scrollerHandledClick = scroller.handleClick(new Point(motionEvent.coordinates.X, motionEvent.coordinates.Y));
                if (!scrollerHandledClick)
                {
                    updateCursorWithMousePos(motionEvent.coordinates);
                    selectionBeganIndex = cursorIndex;
                    makingSelection = true;
                }
                return new MouseHandleResult(motionEvent, HandledStatus.HANDLED, true);
            }
            else if (motionEvent.eventType == MotionType.MOVE)
            {
                bool scrollerHandled = scroller.handleMove(new Point(motionEvent.coordinates.X, motionEvent.coordinates.Y));
                if (makingSelection && !scrollerHandled) updateCursorWithMousePos(motionEvent.coordinates);

            }
            else if (motionEvent.eventType == MotionType.UP)
            {
                if (!singleLine) scroller.handleUp(new Point(motionEvent.coordinates.X, motionEvent.coordinates.Y));
                makingSelection = false;
                return new MouseHandleResult(motionEvent, HandledStatus.HANDLED, false);
            }
            return new MouseHandleResult(motionEvent, HandledStatus.NOT_HANDLED, makingSelection || scroller.isScrolling());
        }

        private void updateCursorWithMousePos(Point pos)
        {
            cursorIndex = getIndexForPos(new Point(pos.X + previousBounds.Left - scroller.scrollX, pos.Y - previousBounds.Top - scroller.scrollY));
        }

        private void updateSelection(Point pos)
        {
            cursorIndex = getIndexForPos(new Point(pos.X + previousBounds.Left - scroller.scrollX, pos.Y - previousBounds.Top - scroller.scrollY));
        }

        public override void draw(DrawCanvas drawCanvas, Rect rect)
        {
            ICanvasRectItem edge = new CanvasItemFactory().createCanvasRectItem();
            edge.setColor(edgeColor);
            edge.setSize(rect.Width, rect.Height);
            drawCanvas.drawToCanvas(edge, rect.Left, rect.Top);

            if (rect.Width - 2 * BORDER_SIZE < 0 || rect.Height - 2 * BORDER_SIZE < 0) return;

            Rect textRegion = new Rect(new Point(rect.Left + BORDER_SIZE, rect.Top + BORDER_SIZE), new Size(rect.Width - 2 * BORDER_SIZE, rect.Height - 2 * BORDER_SIZE));

            drawCanvas.enableClipping((int)textRegion.Left, (int)textRegion.Top, (int)textRegion.Width, (int)textRegion.Height);

            ICanvasRectItem background = new CanvasItemFactory().createCanvasRectItem();
            background.setColor(backgroundColor);
            background.setSize(textRegion.Width, textRegion.Height);
            drawCanvas.drawToCanvas(background, textRegion.X, textRegion.Y);

            calcualteAllTextPositions();

            previousBounds = new Rect(new Point(textRegion.X, textRegion.Y), new Size(textRegion.Width, textRegion.Height));

            int selectionStartIndex = Math.Min(selectionBeganIndex, cursorIndex);
            int selectionEndIndex = Math.Max(selectionBeganIndex, cursorIndex);

            for (int i = 0; i < displayText.Length; i++)
            {
                if (displayText[i] == '\r' || displayText[i] == '\n')
                    continue;

                Point letterPos = getTextPosForIndex(i);
                letterPos.X += textRegion.Left; // Add textBox offset
                letterPos.Y += textRegion.Top;
                letterPos.X += scroller.scrollX; // Add textBox offset
                letterPos.Y += scroller.scrollY;

                if (letterPos.X + charecterDimensions.Width < 0) continue;
                if (letterPos.Y + charecterDimensions.Height < 0) continue;
                if (letterPos.X  > textRegion.Width) continue;
                if (letterPos.Y > textRegion.Height + rect.Top) continue;

                if ((i >= selectionStartIndex && i < selectionEndIndex))
                {
                    double selectionWidth = charecterDimensions.Width;
                    if (displayText[i] == '\t') selectionWidth *= tabSpaces;

                    ICanvasRectItem selection = new CanvasItemFactory().createCanvasRectItem();
                    background.setColor(edgeColor);
                    background.setSize(selectionWidth, charecterDimensions.Height);
                    drawCanvas.drawToCanvas(background, letterPos.X, letterPos.Y);
                }

                if (displayText[i] != '\t')
                {
                    ICanvasTextItem letterCanvasItem = new CanvasItemFactory().createCanvasTextItem();
                    letterCanvasItem.setText(displayText[i] + "");
                    drawCanvas.drawToCanvas(letterCanvasItem, letterPos.X, letterPos.Y);
                }
            }

            if (isViewFocused && (DateTime.Now.Second % 2 == 1 || Keyboard.isAnyKeyDown() || makingSelection))
                drawCursor(drawCanvas, textRegion, charecterDimensions.Width, charecterDimensions.Height);

            if (singleLine)
                scroller.hideBars = true;

            scroller.drawScroller(drawCanvas, textRegion, new Rect(new Size(maxXPos + charecterDimensions.Width, maxYPos + charecterDimensions.Height)));

            drawCanvas.disableClipping();
        }

        public void setText(string text)
        {
            displayText = text;
        }

        public string getText()
        {
            return displayText;
        }

        private void drawCursor(DrawCanvas drawCanvas, Rect bounds, double charWidth, double charHeight)
        {
            Point cursorPos = getTextPosForIndex(getCursorIndex());
            cursorPos.X += bounds.Left; // Add textBox offset
            cursorPos.Y += bounds.Top;
            cursorPos.X += scroller.scrollX; // Add textBox offset
            cursorPos.Y += scroller.scrollY;

            ICanvasRectItem cursorRect = new CanvasItemFactory().createCanvasRectItem();
            cursorRect.setColor(edgeColor);
            cursorRect.setSize(CURSOR_WIDTH, charHeight - 2);
            drawCanvas.drawToCanvas(cursorRect, cursorPos.X + 1, cursorPos.Y + 1);
        }

        private void updateScrollOffsetToKeepCursorOnScreen()
        {
            Point cursorPos = getTextPosForIndex(getCursorIndex());
            
            if (cursorPos.X + 2 * charecterDimensions.Width >= -scroller.scrollX + previousBounds.Right)
                scroller.scrollX = - (cursorPos.X  - previousBounds.Right + 2 * charecterDimensions.Width);
            if (cursorPos.X < -scroller.scrollX)
                scroller.scrollX = - (cursorPos.X );

            if (cursorPos.Y + 2* charecterDimensions.Height > -scroller.scrollY + previousBounds.Height)
                scroller.scrollY = -(cursorPos.Y - previousBounds.Height + 2* charecterDimensions.Height);
            else if (cursorPos.Y < -scroller.scrollY)
                scroller.scrollY = cursorPos.Y;                
        }

        private int getCursorIndex()
        {
            return Math.Max(Math.Min(cursorIndex, displayText.Length), 0);
        }

        private void calcualteAllTextPositions()
        {
            locationsPerIndex.Clear();
            maxYPos = 0;
            maxXPos = 0;

            double x = 0;
            double y = 0;

            locationsPerIndex.Add(new Point(x, y));

            for (int i = 0; i < displayText.Length; i++)
            {
                char currentChar = displayText[i];
                if (currentChar == '\t') x += charecterDimensions.Width * tabSpaces;
                else if (currentChar == '\n')
                {
                    y += charecterDimensions.Height;
                    x = 0;
                }
                else x += charecterDimensions.Width;

                if (x > maxXPos) maxXPos = (int)x;

                locationsPerIndex.Add(new Point(x, y));
            }

            maxYPos = (int)y;

        }

        private Point getTextPosForIndex(int index)
        {
            if (index >= locationsPerIndex.Count) return locationsPerIndex.Last();
            else return locationsPerIndex[index];
        }

        private int getIndexForPos(Point pos)
        {
            if (pos.Y < locationsPerIndex[0].Y) return 0;

            List<Point> yInZone = locationsPerIndex.FindAll((Point point) =>
            {
                if (pos.Y >= point.Y && pos.Y <= point.Y + charecterDimensions.Height)
                    return true;
                else return false;
            });

            if (yInZone.Count == 0) return displayText.Length;

            List<Point> xInZone = locationsPerIndex.FindAll((Point point) =>
            {
                if (pos.X >= point.X && pos.X <= point.X + charecterDimensions.Width) return true;
                else return false;
            });

            var intersect = xInZone.Intersect(yInZone);
            if (intersect.Count() != 0) return locationsPerIndex.IndexOf(intersect.First());

            if (yInZone.Count != 0)
                return locationsPerIndex.IndexOf(yInZone.OrderByDescending(po => po.Y).Last());

            return displayText.Length;
        }
    }
}