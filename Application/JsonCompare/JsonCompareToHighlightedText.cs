using Layout;
using System;
using System.Collections.Generic;
using System.Text;
using static App.JsonCompare.CompareDeserializer;
using static App.JsonCompare.HighlightedTextContents;

namespace App.JsonCompare
{
    public class JsonCompareToHighlightedText
    {
        private int indentStep = 2;

        public List<HighlightedText> convertToHighlightedText(List<CompareItem> objectParameterComparisons)
        {
            List<HighlightedText> highlightedTexts = new List<HighlightedText>();

            addObjectDiffs(highlightedTexts, objectParameterComparisons, false, 0, false);

            return highlightedTexts;
        }

        private void addObjectDiffs(List<HighlightedText> highlightedTexts, List<CompareItem> objectParameterComparisons, bool isArray, int indentAmount, bool isArrayItem)
        {
            if (isArray) highlightedTexts.Add(new HighlightedText() { text = "[\n" });
            else highlightedTexts.Add(new HighlightedText() { text = "{\n" }); // else is an object

            indentAmount += indentStep;

            foreach (CompareItem comp in objectParameterComparisons)
            {
                Color color = new Color(0, 0, 0, 0);

                switch (comp.comparison)
                {
                    case "MODIFIED":
                        color = new Color(50, 75, 200);
                        break;
                    case "ADDED":
                        color = new Color(75, 200, 0);
                        break;
                    case "REMOVED":
                        color = new Color(250, 50, 50);
                        break;
                    case "NO_KEY":
                        color = new Color(125, 125, 125);
                        break;
                }

                switch (comp.type)
                {
                    case "OBJECT":
                        if (comp.comparison == "N/A")
                        {
                            if (!isArrayItem) highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) + "\"" + comp.name + "\"" + ":" });
                            else highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) });
                            addObjectDiffs(highlightedTexts, comp.comparisonObject, false, indentAmount, false);
                        }
                        else
                        {
                            string valueToDisplay = identAllLinesByAdditionalAmount(comp.rValue, indentAmount);
                            if (comp.comparison == "REMOVED")
                                valueToDisplay = identAllLinesByAdditionalAmount(comp.lValue, indentAmount);

                            string objText = new String(' ', indentAmount);
                            if (!isArrayItem) objText += "\"" + comp.name + "\"" + ":";
                            else objText += valueToDisplay.Trim();

                            highlightedTexts.Add(new HighlightedText()
                            {
                                highlightColor = color,
                                text = objText
                            });
                        }
                        break;
                    case "ARRAY":
                        if (!isArrayItem) highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) + "\"" + comp.name + "\"" + ":" });
                        else highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) });
                        addObjectDiffs(highlightedTexts, comp.comparisonObject, true, indentAmount, true);
                        break;
                    case "PRIMITIVE":

                        string displayVal = comp.rValue;
                        if (comp.comparison == "REMOVED") displayVal = comp.lValue;

                        string primitveText = new String(' ', indentAmount);
                        if (!isArrayItem) primitveText += "\"" + comp.name + "\"" + ":" + displayVal;
                        else primitveText += displayVal;

                        highlightedTexts.Add(new HighlightedText()
                        {
                            highlightColor = color,
                            text = primitveText
                        });
                        break;
                    default:
                        throw new Exception("Unsupported parameter type");
                }


                string newLineText;
                if (!isArray) newLineText = ",\n";
                else
                {
                    bool lastItem = comp != objectParameterComparisons[objectParameterComparisons.Count - 1];
                    if (!lastItem) newLineText = ",\n";
                    else newLineText = "\n";
                }

                {
                    highlightedTexts.Add(new HighlightedText()
                    {
                        highlightColor = color,
                        text = newLineText
                    });
                }
            }

            indentAmount -= indentStep;

            if (isArray) highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) + "]" });
            else highlightedTexts.Add(new HighlightedText() { text = new String(' ', indentAmount) + "}" }); // else is an object
        }


        private string identAllLinesByAdditionalAmount(string text, int extraIndent)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] parts = text.Split('\n');
            int i = 0;
            foreach (string line in parts)
            {
                stringBuilder.Append(new String(' ', extraIndent) + line);
                if (i != parts.Length - 1) stringBuilder.Append("\n");
                i++;
            }
            return stringBuilder.ToString();
        }

    }
}
