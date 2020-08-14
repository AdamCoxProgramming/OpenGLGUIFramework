using Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.JsonCompare
{
    public class HighlightedTextContents
    {
        private List<HighlightedText> highlightedText = new List<HighlightedText>();
        private string continuousText = "";
        private List<Color> colorsByIndex = new List<Color>();

        public class HighlightedText
        {
            public string text = "";
            public Color highlightColor = new Color(0, 0, 0, 0);
        }

        public void setHighlightedText(List<HighlightedText> highLightedText)
        {
            this.highlightedText = highLightedText;
            continuousText = getContinuousText(highLightedText);

            colorsByIndex.Clear(); // store colors for text indexs
            foreach (HighlightedText hlText in highlightedText)
                foreach (char leter in hlText.text)
                    colorsByIndex.Add(hlText.highlightColor);
        }

        public string getText()
        {
            return continuousText;
        }

        private string getContinuousText(List<HighlightedText> highlights)
        {
            string res = "";

            foreach (HighlightedText section in highlights)
            {
                res += section.text;
            }
            return res;
        }

        public Color getColorForCharAtIndex(int index)
        {
            return colorsByIndex[index];
        }

    }

}
