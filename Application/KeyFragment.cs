using Layout;
using LayoutFramework;
using LayoutFramework.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Layout.LayoutImplementations;
using static LayoutFramework.Layouts.LinearLayout;

namespace App
{
    class KeyFragment : Fragment
    {
        private Action onKeyChanged = null;
        private Action onKeyDeletePressed = null;

        private EditTextLayout keyEditText = null;

        private Color buttonRed = new Color(174, 79, 79);

        public override BaseLayout getContent()
        {
            return createKeyLayout();
        }

        private BaseLayout createKeyLayout()
        {
            int marginSize = 10;

            LinearLayout horizontalStack = new LinearLayout();
            horizontalStack.setDirection(LinearLayout.Direction.HORIZONTAL);
            horizontalStack.setSizeParams(new SizeParams(FILL, WRAP_CONTENTS));
            horizontalStack.invertDirection = true;

            ButtonLayout deleteButton = new ButtonLayout();
            deleteButton.setSizeParams(new SizeParams(25, MATCH_PARENT));
            deleteButton.color = buttonRed;

            TextBoxLayout delText = new TextBoxLayout();
            delText.setSizeParams(new SizeParams(WRAP_CONTENTS,WRAP_CONTENTS));
            delText.text = "X";
            delText.textSize = 11;

            deleteButton.setContents(delText);

            ColoredLayout blankLeftMargin = new ColoredLayout();
            blankLeftMargin.color.a = 0;
            blankLeftMargin.setSizeParams(new SizeParams(marginSize, 0));

            keyEditText = new EditTextLayout();
            keyEditText.edgeColor = LayoutLoader.buttonBlue;
            keyEditText.setSizeParams(new SizeParams(FILL, 22));
            keyEditText.singleLine = true;
            keyEditText.susbcribeToTextChanges(() =>
            {
                if (onKeyChanged != null) onKeyChanged();
            });

            horizontalStack.addItem(blankLeftMargin);
            horizontalStack.addItem(deleteButton);

            horizontalStack.addItem(blankLeftMargin);
            horizontalStack.addItem(keyEditText);

            LinearLayout leftLay = new LinearLayout();
            leftLay.setSizeParams(new SizeParams(FILL, WRAP_CONTENTS));
            leftLay.setDirection(Direction.HORIZONTAL);
            leftLay.addItem(blankLeftMargin);
            leftLay.addItem(horizontalStack);

            ColoredLayout blankTopMargin = new ColoredLayout();
            blankTopMargin.color.a = 0;
            blankTopMargin.setSizeParams(new SizeParams(0, marginSize));

            LinearLayout topLay = new LinearLayout();
            topLay.setSizeParams(new SizeParams(FILL, WRAP_CONTENTS));
            topLay.setDirection(Direction.VERTICAL);
            topLay.invertDirection = true;

            topLay.addItem(blankTopMargin);
            topLay.addItem(leftLay);

            deleteButton.metaData = topLay;
            deleteButton.setOnClickListener(() =>
            {
                if (onKeyDeletePressed != null) onKeyDeletePressed();
            });

            return topLay;
        }

        public string getKeyText()
        {
            return keyEditText.getText();
        }

        public void subscribeTextChanges(Action callback)
        {
            onKeyChanged = callback;
        }

        public void subscribeToDeletePress(Action callback)
        {
            onKeyDeletePressed = callback;
        }

    }
}
