using System;
using System.Windows;
using BasicWindow.Resources;
using Layout;
using LayoutFramework.CanvasItems;
using OpenTK;

namespace BasicWindow
{
    public class TextCanvasItem : ICanvasItem, IGLCanvasItem, ICanvasTextItem
    {
        private double x, y;

        private static TextRenderer textRenderer = new TextRenderer();

        private String textToRender = "";

        private double fontSize = 8.6;

        private Color color = new Color(0, 0, 0);

        void IGLCanvasItem.render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            textRenderer.setColor(color);
            textRenderer.setText(textToRender);
            textRenderer.setTextSize(fontSize);
            textRenderer.setLocation(new Point(x, y));
            textRenderer.render(viewMatrix, projectionMatrix);
        }

        public Rect measureText(string text)
        {
            return new Rect(new Size(text.Length * 1.0f * TextRenderer.letterDimensionRatio.X * fontSize, TextRenderer.letterDimensionRatio.Y * fontSize));
        }

        public void setText(string text)
        {
            this.textToRender = text;
        }

        void ICanvasItem.setTopLeft(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void setTextSize(double fontSize)
        {
            this.fontSize = fontSize;
        }

        public void setTextColor(Color color)
        {
            this.color = color;
        }
    }
}
