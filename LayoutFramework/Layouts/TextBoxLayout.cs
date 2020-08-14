using Layout;
using LayoutFramework.CanvasItems;
using System.Windows;
using static Layout.EventHandler;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.LayoutRenderer;

namespace LayoutFramework.Layouts
{
    public class TextBoxLayout : BaseLayout
    {
        public Color backgroundColor = new Color(255, 255, 255);
        public string text = "";
        public Color textColor = new Color(0,0,0);

        public double textSize = 7;

        ICanvasTextItem textMeasurer;

        public TextBoxLayout()
        {
            textMeasurer = new CanvasItemFactory().createCanvasTextItem();
        }

        protected override MeasuredLayout calculateMeasuredLayout(Bounds perantBounds)
        {
            MeasuredLayout calculatedItem = new MeasuredLayout();
            calculatedItem.drawable = this;
            calculatedItem.reactiveView = this;

            textMeasurer.setTextSize(textSize);
            Rect textDimensions = textMeasurer.measureText(text);

            calculatedItem.setBounds(calculateBounds(perantBounds.rect, new Rect(new Size(textDimensions.Width, textDimensions.Height))));

            return calculatedItem;
        }

        public override void draw(DrawCanvas drawCanvas, Rect rect)
        {
            drawCanvas.enableClipping((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);

            ICanvasTextItem textCanvasItem = new CanvasItemFactory().createCanvasTextItem();
            textCanvasItem.setText(text);
            textCanvasItem.setTextColor(textColor);
            textCanvasItem.setTextSize(textSize);
            drawCanvas.drawToCanvas(textCanvasItem, rect.Left, rect.Top);

            drawCanvas.disableClipping();
        }
    }
}
