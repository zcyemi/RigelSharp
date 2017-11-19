using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;
using SharpDX;

namespace RigelEditor.EGUI
{
    internal static class GUITextProcessor
    {

        private static readonly Dictionary<KeyCode, char> KeyMapLower = new Dictionary<KeyCode, char>()
        {
            {KeyCode.Oem1,';'},
            {KeyCode.Oem7,'\'' },
            {KeyCode.Oem5,'\\' },
            {KeyCode.OemOpenBrackets,'[' },
            {KeyCode.OemCloseBrackets,']' },
            {KeyCode.OemMinus,'-' },
            {KeyCode.Oemplus,'=' },
            {KeyCode.OemPeriod,'.' },
            {KeyCode.Oemcomma,',' },
            {KeyCode.OemQuestion,'/' },
            {KeyCode.Oemtilde,'`' }
        };

        private static readonly Dictionary<KeyCode, char> KeyMapUpper = new Dictionary<KeyCode, char>()
        {
            {KeyCode.Oem1,':'},
            {KeyCode.Oem7,'\"' },
            {KeyCode.Oem5,'|' },
            {KeyCode.OemOpenBrackets,'{' },
            {KeyCode.OemCloseBrackets,'}' },
            {KeyCode.OemMinus,'_' },
            {KeyCode.Oemplus,'+' },
            {KeyCode.OemPeriod,'<' },
            {KeyCode.Oemcomma,'>' },
            {KeyCode.OemQuestion,'?' },
            {KeyCode.Oemtilde,'~' }
        };

        private static readonly string DKey = ")!@#$%^&*()";

        public static string ProcessInput(string text, KeyCode key, ref int pos)
        {
            switch (key)
            {
                case KeyCode.Back:
                    if (pos > 0)
                    {
                        text = text.Remove(pos - 1, 1);
                        pos--;
                    }
                    break;
                case KeyCode.Space:
                    text = text.Insert(pos, " ");
                    pos++;
                    break;
                case KeyCode.Left:
                    pos--;
                    break;
                case KeyCode.Right:
                    pos++;
                    break;
            }

            pos = MathUtil.Clamp(pos, 0, text.Length);

            int keyv = (int)key;

            string keystr = KeyToString(key, GUI.Event.Shift);
            if (!string.IsNullOrEmpty(keystr))
            {
                text = text.Insert(pos, keystr);
                pos += 1;
            }
            return text;
        }

        public static string KeyToString(KeyCode key,bool shift = false)
        {
            int keyv = (int)key;

            if ((int)KeyCode.A <= keyv && keyv <= (int)KeyCode.Z)
            {
                return shift ? key.ToString().ToUpper() : key.ToString().ToLower();
            }
            if (keyv >= 96 && keyv <= 105)
            {
                return (keyv - 90).ToString();
            }

            if (KeyMapLower.ContainsKey(key))
            {
                return (shift ? KeyMapUpper[key] : KeyMapLower[key]).ToString();
            }

            if(keyv >=48 && keyv <= 57)
            {
                if (shift)
                {
                    return DKey[keyv - 48].ToString();
                }
                else
                {
                    return (keyv - 48).ToString();
                }
            }
            return null;
        }
    }
}
