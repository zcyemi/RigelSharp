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
        
        internal static int s_layoutLineHeight = 25;
        internal static int s_layoutLineIndent = 5;

        //for horizontal or vertical layout
        internal static Vector2 s_layoutOffset;
        internal static Stack<Vector2> s_layoutStack = new Stack<Vector2>();
        internal static Stack<bool> s_layoutStackType = new Stack<bool>();
        internal static bool s_layoutVertical = true;
        internal static Vector2 s_layoutMax;

        internal static void AutoCaculateOffset(int w, int h)
        {
            if (s_layoutVertical)
            {
                s_layoutOffset.Y += h;
            }
            else
            {
                s_layoutOffset.X += w;
            }
        }

        internal static void AutoCaculateOffsetW(int w)
        {
            AutoCaculateOffset(w, s_layoutLineHeight);
        }


        internal static void AutoCaculateOffsetH(int h)
        {
            AutoCaculateOffset(s_layoutLineIndent,h);
        }


        public static bool Button(string label)
        {
            var ret = RigelEGUI.Button(new Vector4(s_layoutOffset, 50f, 20), label, RigelEGUIStyle.Current.ButtonColor, Vector4.One);

            AutoCaculateOffsetW(50);

            return ret;
        }

        public static void Text(string content)
        {
            var width = RigelEGUI.DrawText(new Vector4(s_layoutOffset, 400, s_layoutLineHeight), content, Vector4.One);

            AutoCaculateOffsetW(width);
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



    }


    internal static class RigelEGUILayoutTest
    {
        internal static void LayoutBasic()
        {
            RigelEGUILayout.Button("Button1");
            RigelEGUILayout.Button("Button2");

            RigelEGUILayout.BeginHorizontal();

            RigelEGUILayout.Button("Button3");
            RigelEGUILayout.Button("Button4");

            RigelEGUILayout.BeginVertical();

            RigelEGUILayout.Button("Button5");

            RigelEGUILayout.BeginHorizontal();
            RigelEGUILayout.Button("Button5v");
            RigelEGUILayout.Button("Button5v1");
            RigelEGUILayout.EndHorizontal();
            RigelEGUILayout.Button("Button5v");
            RigelEGUILayout.EndVertical();

            RigelEGUILayout.Button("Button4");


            RigelEGUILayout.EndHorizontal();

            RigelEGUILayout.Button("Button6");

            RigelEGUILayout.Button("Button7");
        }

        static bool s_bLayoutButtonTab = true;
        internal static void LayoutButtonTab()
        {
            RigelEGUILayout.BeginHorizontal();
            if (RigelEGUILayout.Button("Tab1"))
            {
                s_bLayoutButtonTab = true;
            }
            if (RigelEGUILayout.Button("Tab2"))
            {
                s_bLayoutButtonTab = false;
            }
            RigelEGUILayout.EndHorizontal();
            if (s_bLayoutButtonTab)
            {
                RigelEGUILayout.Button("AAAA");
            }
            else
            {
                RigelEGUILayout.Button("BBBB");
            }
            
        }

        internal static void LayoutTextSample1()
        {
            RigelEGUILayout.Text("Hello world!");

            RigelEGUILayout.BeginHorizontal();

            RigelEGUILayout.Text("www.google.com");

            RigelEGUILayout.Button("Enter");

            RigelEGUILayout.BeginVertical();
            RigelEGUILayout.Text("Test");
            RigelEGUILayout.Text("Test--------[]");
            RigelEGUILayout.EndVertical();

            RigelEGUILayout.Text("AnotherLine");

            RigelEGUILayout.EndHorizontal();
        }
    }
}
