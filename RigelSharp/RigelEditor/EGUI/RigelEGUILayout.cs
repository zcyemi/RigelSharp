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


        internal static Stack<LayoutInfo> s_layoutStack = new Stack<LayoutInfo>();
        internal static LayoutInfo s_layout;

        internal static Stack<Vector4> s_areaStack = new Stack<Vector4>();
        internal static Vector4 s_area;

        public struct LayoutInfo
        {
            public bool Verticle;
            public Vector2 Offset;
            public Vector2 SizeMax;

            public float DefaultHeight;
            public float DefaultWidth;

        }


        internal static void AutoCaculateOffset(int w, int h)
        {
            if (s_layout.Verticle)
            {
                s_layout.Offset.Y += h;
                s_layout.SizeMax.X = Math.Max(s_layout.SizeMax.X, w);
            }
            else
            {
                s_layout.Offset.X += w;
                s_layout.SizeMax.Y = Math.Max(s_layout.SizeMax.Y, h);
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

        internal static void Frame(int width,int height)
        {
            s_layout.Offset = Vector2.Zero;
            s_layout.Verticle = true;
            s_layout.SizeMax = Vector2.Zero;
            RigelEGUI.SetDepthBase(0);

            s_layoutStack.Clear();
            s_layoutStack.Push(s_layout);

            s_areaStack.Clear();
            s_area = new Vector4(0, 0, width, height);

        }


        public static void BeginMenuBar()
        {
            var rect = new Vector4(0, 0, s_area.Z, 20);
            BeginArea(rect);
            RigelEGUI.DrawRect(rect, RigelEGUIStyle.Current.MainMenuBGColor);
            BeginHorizontal();
        }

        public static void MenuItem()
        {

        }

        public static void EndMenuBar()
        {
            EndHorizontal();
            EndArea();
        }

        public static bool Button(string label)
        {
            var rect = new Vector4(s_layout.Offset, 50f, 20);
            rect.X += s_area.X;
            rect.Y += s_area.Y;

            var ret = RigelEGUI.Button(rect, label, RigelEGUIStyle.Current.ButtonColor, Vector4.One);

            AutoCaculateOffsetW(50);

            return ret;
        }

        public static void Text(string content)
        {
            var rect = new Vector4(s_layout.Offset, 400, s_layoutLineHeight);
            rect.X += s_area.X;
            rect.Y += s_area.Y;
            var width = RigelEGUI.DrawText(rect, content, Vector4.One);

            AutoCaculateOffsetW(width);
        }

        public static void BeginHorizontal()
        {
            s_layout.Verticle = false;
            s_layoutStack.Push(s_layout);

            s_layout.SizeMax.Y = 0;
        }

        public static void EndHorizontal()
        {
            var playout = s_layoutStack.Pop();
            s_layout.Verticle = s_layoutStack.Peek().Verticle;

            var lastOffset = playout.Offset;

            var off = s_layout.Offset - lastOffset;
            s_layout.SizeMax.X = Math.Max(off.X, s_layout.SizeMax.X);
            s_layout.Offset.Y += s_layout.SizeMax.Y > s_layoutLineHeight ? s_layout.SizeMax.Y : s_layoutLineHeight;
            s_layout.Offset.X = lastOffset.X;
        }

        public static void BeginVertical()
        {
            s_layout.Verticle = true;
            s_layoutStack.Push(s_layout);
            s_layout.SizeMax.X = 0;
        }
        public static void EndVertical()
        {
            var playout = s_layoutStack.Pop();
            s_layout.Verticle = s_layoutStack.Peek().Verticle;
            var lastOffset = playout.Offset;

            var off = s_layout.Offset - lastOffset;

            s_layout.SizeMax.Y = Math.Max(off.Y, s_layout.SizeMax.Y);

            s_layout.Offset.X += s_layout.SizeMax.X > 5f ? s_layout.SizeMax.X : 5f;
            s_layout.Offset.Y = lastOffset.Y;
        }

        public static void Space()
        {
            s_layout.Offset.Y += s_layoutLineHeight;
        }
        public static void Space(int height)
        {
            s_layout.Offset.Y += height;
        }

        public static void Indent()
        {
            s_layout.Offset.X += s_layoutLineIndent;
        }
        public static void Indent(int width)
        {
            s_layout.Offset.X += width;
        }

        public static void BeginArea(Vector4 rect)
        {
            s_areaStack.Push(rect);
            s_area = rect;

            s_layoutStack.Push(s_layout);

            s_layout.Verticle = true;
            s_layout.SizeMax = Vector2.Zero;
            s_layout.Offset = Vector2.Zero;
        }

        public static void EndArea()
        {
            s_areaStack.Pop();

            s_layoutStack.Pop();
            s_layout = s_layoutStack.Peek();
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

            RigelEGUILayout.BeginVertical();
            RigelEGUILayout.Text("Test");
            RigelEGUILayout.Space();
            
            RigelEGUILayout.Text("Test--------[]");
            RigelEGUILayout.EndVertical();

            RigelEGUILayout.Indent();
            RigelEGUILayout.Button("Enter");


            RigelEGUILayout.Text("AnotherLine");

            RigelEGUILayout.EndHorizontal();
        }

        internal static void LayoutAreaSample()
        {
            RigelEGUILayout.Button("Test1");
            RigelEGUILayout.Button("Test2");

            RigelEGUILayout.BeginArea(new Vector4(30, 30, 100, 100));

            RigelEGUILayout.Button("Test Area");
            RigelEGUILayout.EndArea();

            RigelEGUILayout.Button("Test3");

            
        }
    }
}
