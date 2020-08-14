using System;

namespace LayoutFramework.Input
{
    class GetCharForPressedKey
    {
        public char getCharForKey(Keys.Key keyPressed)
        {
            bool isShiftDown = Keyboard.isShiftDown();

            char charPressed = ' ';

            if (keyPressed >= Keys.Key.A && keyPressed <= Keys.Key.Z)
            {
                char letterPressed = (char)('a' + (keyPressed - Keys.Key.A));

                if (isShiftDown)
                    letterPressed = Char.ToUpper(letterPressed);

                charPressed = letterPressed;
            }
            else if (keyPressed >= Keys.Key.Number0 && keyPressed <= Keys.Key.Number9)
            {
                char letterPressed = (char)('1' + (keyPressed - Keys.Key.Number1));

                if (!isShiftDown)
                    charPressed = letterPressed;
                else
                {
                    switch (keyPressed)
                    {
                        case Keys.Key.Number0:
                            charPressed = ')';
                            break;
                        case Keys.Key.Number1:
                            charPressed = '!';
                            break;
                        case Keys.Key.Number2:
                            charPressed = '"';
                            break;
                        case Keys.Key.Number3:
                            charPressed = '£';
                            break;
                        case Keys.Key.Number4:
                            charPressed = '$';
                            break;
                        case Keys.Key.Number5:
                            charPressed = '%';
                            break;
                        case Keys.Key.Number6:
                            charPressed = '^';
                            break;
                        case Keys.Key.Number7:
                            charPressed = '&';
                            break;
                        case Keys.Key.Number8:
                            charPressed = '*';
                            break;
                        case Keys.Key.Number9:
                            charPressed = '(';
                            break;
                    }
                }
            }
            else if (keyPressed == Keys.Key.NonUSBackSlash)
            {
                if (isShiftDown) charPressed = '|';
                else charPressed = '\\';
            }
            else if (keyPressed == Keys.Key.Comma)
            {
                if (isShiftDown) charPressed = '<';
                else charPressed = ',';
            }
            else if (keyPressed == Keys.Key.Period)
            {
                if (isShiftDown) charPressed = '>';
                else charPressed = '.';
            }
            else if (keyPressed == Keys.Key.Slash)
            {
                if (isShiftDown) charPressed = '?';
                else charPressed = '/';
            }
            else if (keyPressed == Keys.Key.Semicolon)
            {
                if (isShiftDown) charPressed = ':';
                else charPressed = ';';
            }
            else if (keyPressed == Keys.Key.Quote)
            {
                if (isShiftDown) charPressed = '@';
                else charPressed = '\'';
            }
            else if (keyPressed == Keys.Key.BackSlash)
            {
                if (isShiftDown) charPressed = '~';
                else charPressed = '#';
            }
            else if (keyPressed == Keys.Key.BracketLeft)
            {
                if (isShiftDown) charPressed = '{';
                else charPressed = '[';
            }
            else if (keyPressed == Keys.Key.BracketRight)
            {
                if (isShiftDown) charPressed = '}';
                else charPressed = ']';
            }
            else if (keyPressed == Keys.Key.Grave)
            {
                if (isShiftDown) charPressed = '¬';
                else charPressed = '`';
            }
            else if (keyPressed == Keys.Key.Minus)
            {
                if (isShiftDown) charPressed = '_';
                else charPressed = '-';
            }
            else if (keyPressed == Keys.Key.Plus)
            {
                if (isShiftDown) charPressed = '+';
                else charPressed = '=';
            }

            return charPressed;
        }
    }
}
