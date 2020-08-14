using System.Collections.Generic;

namespace LayoutFramework
{
    public class Keyboard
    {
        private static Dictionary<Keys.Key,bool> keysDown = new Dictionary<Keys.Key, bool>();

        public static void notifyKeyPressed(Keys.Key keyPressed)
        {
            keysDown[keyPressed] = true;
        }

        public static void notifyKeyUp(Keys.Key keyReleased)
        {
            keysDown[keyReleased] = false;
        }

        public static bool isKeyDown(Keys.Key key)
        {            
            if(keysDown.ContainsKey(key) )
            {
                bool hasKey;
                keysDown.TryGetValue(key, out hasKey);
                return hasKey;
            }
            return false;
        }

        public static bool isShiftDown()
        {
            return isKeyDown(Keys.Key.ShiftLeft) || isKeyDown(Keys.Key.ShiftRight);
        }

        public static bool isControlDown()
        {
            return isKeyDown(Keys.Key.ControlLeft) || isKeyDown(Keys.Key.ControlRight);
        }

        public static bool isAnyKeyDown()
        {
            foreach (KeyValuePair<Keys.Key, bool> entry in keysDown)
            {
                if (entry.Value == true) return true;
            }
            return false;
        }

    }
}
