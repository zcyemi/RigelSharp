using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public static class RigelEGUILayout
    {
        
        internal static float s_layoutLineHeight = 25f;

        internal static bool s_horizontal = false;

        internal static Vector2 s_layoutOffset;
        internal static Stack<Vector2> s_layoutStack = new Stack<Vector2>();
        internal static Stack<bool> s_layoutStackType = new Stack<bool>();

        internal static bool s_layoutVertical = true;
        internal static Vector2 s_layoutMax;


        public static void BeginArea(Vector4 rect)
        {

        }
        public static void EndArea()
        {

        }



        public static bool Button(string label)
        {
            var ret = RigelEGUI.Button(new Vector4(s_layoutOffset, 50f, 20), label, RigelEGUIStyle.Current.ButtonColor, Vector4.One);

            if (s_layoutVertical)
            {
                s_layoutOffset.Y += s_layoutLineHeight;
            }
            else
            {
                s_layoutOffset.X += 55f;
            }

            return ret;
        }

        public static void BeginHorizontal()
        {
            s_layoutStackType.Push(false);
            s_layoutStack.Push(s_layoutOffset);

            s_layoutVertical = false;
            s_layoutMax.Y = 0;
        }

        public static void EndHorizontal()
        {
            s_layoutStackType.Pop();
            s_layoutVertical = s_layoutStackType.Peek();
            var lastOffset = s_layoutStack.Pop();

            var off = s_layoutOffset - lastOffset;
            s_layoutMax.X = Math.Max(off.X, s_layoutMax.X);

            s_layoutOffset.Y += s_layoutMax.Y > s_layoutLineHeight ? s_layoutMax.Y : s_layoutLineHeight;
            s_layoutOffset.X = lastOffset.X;
        }

        public static void BeginVertical()
        {
            s_layoutStackType.Push(true);
            s_layoutStack.Push(s_layoutOffset);
            s_layoutVertical = true;
            s_layoutMax.X = 0;
        }
        public static void EndVertical()
        {
            s_layoutStackType.Pop();
            s_layoutVertical = s_layoutStackType.Peek();
            var lastOffset = s_layoutStack.Pop();

            var off = s_layoutOffset - lastOffset;

            s_layoutMax.Y = Math.Max(off.Y, s_layoutMax.Y);

            s_layoutOffset.X += s_layoutMax.X > 5f ? s_layoutMax.X : 5f;
            s_layoutOffset.Y = lastOffset.Y;
        }

        public static void Text()
        {

        }

    }
}
