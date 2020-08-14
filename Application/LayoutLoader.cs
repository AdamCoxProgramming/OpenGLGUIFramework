using App.JsonCompare;
using Layout;
using LayoutFramework.Layouts;
using System.Collections.Generic;
using static App.JsonCompare.CompareDeserializer;
using static Layout.LayoutImplementations;
using static Layout.LayoutImplementations.BaseLayout;
using static LayoutFramework.Layouts.LinearLayout;

namespace App
{
    public class LayoutLoader
    {
        private LinearLayout keyList;
        private LinearLayout contentHolder;
        private LinearLayout jsonInput;
        private JsonLayout jsonResultLayout;

        private Color uiWhite = new Color(235,235,235);
        public static Color buttonBlue = new Color(29, 116, 174);

        private Model model = new Model();

        private bool inputMode = true;

        public BaseLayout loadLayout()
        {            
            BaseLayout rootLayout =  createView();
            model.subscribeToNewResults(() =>
            {
                List<CompareItem> objectComparisons = model.getLastresultValidResult();
                jsonResultLayout.setText(new JsonCompareToHighlightedText().convertToHighlightedText(objectComparisons));
            });
            model.findDifferences();
            return rootLayout; 
        }

        private BaseLayout createView()
        {
            CustomPlacementLayout backgroundFrame = new CustomPlacementLayout();
            backgroundFrame.setSizeParams(new SizeParams(MATCH_PARENT,MATCH_PARENT));
            ColoredLayout background = new ColoredLayout();
            background.color = new Color(45,45,48);
            background.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));
            backgroundFrame.addChild(background);

            ColoredLayout horizontalDivider = new ColoredLayout();
            horizontalDivider.setSizeParams(new SizeParams(MATCH_PARENT,2));
            horizontalDivider.color = new Color(63,63,70);

            ColoredLayout blankSpace = new ColoredLayout();
            blankSpace.setSizeParams(new SizeParams(MATCH_PARENT, 6));
            blankSpace.color = new Color(0, 0, 0, 0);

            LinearLayout vertical = new LinearLayout();
            vertical.setSizeParams(new SizeParams(MATCH_PARENT,MATCH_PARENT));
            vertical.addChild(blankSpace);
            vertical.addChild(createHeader());
            vertical.addChild(blankSpace);
            vertical.addChild(horizontalDivider);
            vertical.addChild(createBody());

            backgroundFrame.addChild(vertical);

            return backgroundFrame;
        }

        private BaseLayout createHeader()
        {
            LinearLayout horizontal = new LinearLayout();
            horizontal.setDirection(LinearLayout.Direction.HORIZONTAL);
            horizontal.setSizeParams(new SizeParams(MATCH_PARENT, WRAP_CONTENTS));

            TextBoxLayout logoText = new TextBoxLayout();
            logoText.text = "{DIFF}";
            logoText.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            logoText.textColor = uiWhite;
            logoText.textSize = 10;

            ColoredLayout blankSpace = new ColoredLayout();
            blankSpace.setSizeParams(new SizeParams(5,MATCH_PARENT));
            blankSpace.color = new Color(0, 0, 0, 0);

            TextBoxLayout headerText = new TextBoxLayout();
            headerText.text = "JSON Comparison Tool";
            headerText.textColor = new Color(150,150,150);
            headerText.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            headerText.setPositionParams(new PositionParams(CENTER,CENTER));

            horizontal.addChild(blankSpace);
            horizontal.addChild(logoText);
            horizontal.addChild(blankSpace);
            horizontal.addChild(headerText);

            return horizontal;

        }

        private BaseLayout createBody()
        {
            LinearLayout horizontal = new LinearLayout();
            horizontal.setDirection(LinearLayout.Direction.HORIZONTAL);
            horizontal.setSizeParams(new SizeParams(MATCH_PARENT, FILL));

            CustomPlacementLayout keysHeader = new CustomPlacementLayout();
            keysHeader.setSizeParams(new SizeParams(MATCH_PARENT,30));

            TextBoxLayout headerText = new TextBoxLayout();
            headerText.text = "Comparison Keys";
            headerText.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            headerText.setPositionParams( new PositionParams( 5, 4));
            headerText.textColor = uiWhite;

            keysHeader.addChild(headerText);

            ColoredLayout greyBody = new ColoredLayout();
            greyBody.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));
            greyBody.color = new Color(90, 90, 90);

            ColoredLayout dargerGreyBody = new ColoredLayout();
            dargerGreyBody.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));
            dargerGreyBody.color = new Color(70, 70, 70);

            SizeParams thirdWidthSizeParams = new SizeParams();
            thirdWidthSizeParams.Width = 250;
            thirdWidthSizeParams.Height = MATCH_PARENT;

            LinearLayout keysColumn = new LinearLayout();
            keysColumn.setSizeParams(thirdWidthSizeParams);
            keysColumn.addItem(keysHeader);
            keysColumn.addItem(createKeysLayout());
            horizontal.addItem(keysColumn);

            BaseLayout switchableSection = createContentLayout();
            switchableSection.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));

            horizontal.addItem(switchableSection);

            return horizontal;
        }


        private BaseLayout createContentLayout()
        {
            contentHolder = new LinearLayout();
            contentHolder.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));
            contentHolder.invertDirection = true;

            contentHolder.addItem(createSwitchViewLayout());

            SizeParams fityPercentWidthSizeParams = new SizeParams();
            fityPercentWidthSizeParams.WidthPercent = 50.0f;
            fityPercentWidthSizeParams.Height = MATCH_PARENT;

            jsonInput = new LinearLayout();
            jsonInput.setSizeParams(new SizeParams(MATCH_PARENT, MATCH_PARENT));
            jsonInput.setDirection(Direction.HORIZONTAL);

            LinearLayout json1Column = new LinearLayout();
            json1Column.setSizeParams(fityPercentWidthSizeParams);
            json1Column.addItem(createJsonHeaderLayout("Left JSON"));
            EditTextLayout json1Text = new EditTextLayout();
            json1Text.edgeColor = new Color(0, 122, 204);
            json1Text.setSizeParams(new SizeParams(MATCH_PARENT, FILL));
            json1Column.addItem(json1Text);
            json1Text.susbcribeToTextChanges(() =>
            {
                model.setjson1(json1Text.getText());
            });
            json1Text.setText(Properties.Resources.TextFile1);

            LinearLayout json2Column = new LinearLayout();
            json2Column.setSizeParams(fityPercentWidthSizeParams);
            json2Column.addItem(createJsonHeaderLayout("Right JSON"));
            EditTextLayout json2Text = new EditTextLayout();
            json2Text.edgeColor = new Color(0, 122, 204);
            json2Text.setSizeParams(new SizeParams(MATCH_PARENT, FILL));
            json2Column.addItem(json2Text);
            json2Text.susbcribeToTextChanges(() =>
            {
                model.setjson2(json2Text.getText());
            });
            json2Text.setText(Properties.Resources.TextFile1);

            jsonInput.addItem(json1Column);
            jsonInput.addItem(json2Column);

            jsonResultLayout = new JsonLayout();

            contentHolder.addItem(jsonInput);

            return contentHolder;
        }        

        private BaseLayout createJsonHeaderLayout(string text)
        {
            Color headerBlue = new Color(0, 122, 204);

            LinearLayout verticalLayout = new LinearLayout();
            verticalLayout.setSizeParams(new SizeParams(MATCH_PARENT, WRAP_CONTENTS));

            CustomPlacementLayout space = new CustomPlacementLayout();
            space.setSizeParams(new SizeParams(100, 20));
            space.color = headerBlue;

            TextBoxLayout headerText = new TextBoxLayout();
            headerText.text = text;
            headerText.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            headerText.setPositionParams( new PositionParams( 5, CENTER));
            headerText.textColor = uiWhite;

            space.addChild(headerText);

            ColoredLayout horizontalDivider = new ColoredLayout();
            horizontalDivider.setSizeParams(new SizeParams(MATCH_PARENT, 3));
            horizontalDivider.color = headerBlue;

            verticalLayout.addChild(space);
            verticalLayout.addChild(horizontalDivider);

            return verticalLayout;
        }

        private BaseLayout createSwitchViewLayout()
        {
            CustomPlacementLayout space = new CustomPlacementLayout();
            space.setSizeParams(new SizeParams(MATCH_PARENT, 125));
            space.color = new Color(37,37,38);

            ButtonLayout button = new ButtonLayout();
            button.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            button.setPositionParams(new PositionParams(CENTER, CENTER));
            button.color = buttonBlue;

            TextBoxLayout text = new TextBoxLayout();
            text.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            text.text = "See Results";
            text.setPositionParams(new PositionParams(CENTER, CENTER));
            text.textColor = uiWhite;

            button.setContents(text);
            button.setOnClickListener(() =>
            {
                if (inputMode)
                {
                    contentHolder.removeChild(jsonInput);
                    contentHolder.addChild(jsonResultLayout);
                    text.text = "Change Input";
                }
                else
                {
                    contentHolder.removeChild(jsonResultLayout);
                    contentHolder.addChild(jsonInput);
                    text.text = "See Results";
                }

                inputMode = !inputMode;
            });

            space.AddItem(button);

            return space;

        }

        private LinearLayout createKeysLayout()
        {
            LinearLayout keyVertical = new LinearLayout();
            keyVertical.setSizeParams(new SizeParams(MATCH_PARENT,MATCH_PARENT));
            keyVertical.setDirection(Direction.HORIZONTAL);
            keyVertical.invertDirection = true;

            LinearLayout linearLayout = new LinearLayout();
            linearLayout.setSizeParams(new SizeParams(FILL, FILL));

            keyList = new LinearLayout();
            keyList.setSizeParams(new SizeParams(MATCH_PARENT, WRAP_CONTENTS));

            linearLayout.addItem(keyList);

            ButtonLayout addButton = new ButtonLayout();
            addButton.setSizeParams(new SizeParams(30, 30));
            addButton.color = new Color(50, 50, 50);
            addButton.setPositionParams(new PositionParams(CENTER, CENTER));
            addButton.color = buttonBlue;

            TextBoxLayout text = new TextBoxLayout();
            text.setSizeParams(new SizeParams(WRAP_CONTENTS, WRAP_CONTENTS));
            text.text = "Add Key";
            text.setPositionParams(new PositionParams(CENTER, CENTER));
            text.textColor = uiWhite;

            addButton.setOnClickListener(() =>
            {
                addKey();
            });

            addKey();

            linearLayout.addItem(addButton);

            ColoredLayout verticalDivider = new ColoredLayout();
            verticalDivider.setSizeParams(new SizeParams(2, MATCH_PARENT));
            verticalDivider.color = new Color(63, 63, 70);

            keyVertical.addChild(verticalDivider);
            keyVertical.addChild(linearLayout);

            return keyVertical;
        }

        private void addKey()
        {
            KeyFragment keyFrag = new KeyFragment();
            keyFrag.subscribeTextChanges(() =>
            {
                model.setKeys(getKeysFromGUI());
            });
            keyFrag.subscribeToDeletePress(() =>
            {
                keyList.removeChild(keyFrag);
                model.setKeys(getKeysFromGUI());
            });
            keyList.addItem(keyFrag);
        }

        private string[] getKeysFromGUI()
        {
            List<string> keys = new List<string>();
            foreach (KeyFragment item in keyList.getChildren())
            {
                keys.Add(item.getKeyText());
            }
            return keys.ToArray();
        }

    }
}
