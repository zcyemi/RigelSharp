using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public static class RigelEGUI
    {
        internal static RigelEGUICtx InternalContext { get; private set; }
        internal static RigelEGUIWindow CurrentWindow { get; private set; }
        internal static float s_depthz = 1;
        internal static float s_depthStep = 0.0001f;
        public static RigelEGUIEvent Event { get; private set; }

        private static Stack<Vector4> s_groupStack = new Stack<Vector4>();
        private static int s_windowGroupCount = 0;

        internal static void InternalResetContext(RigelEGUICtx ctx)
        {
            InternalContext = ctx;
            s_groupStack.Clear();
        }
        internal static void InternalFrameBegin(RigelEGUIEvent e)
        {
            Event = e;
        }
        internal static void InternalFrameEnd()
        {

        }
        internal static void InternalWindowBegin(RigelEGUIWindow win)
        {
            CurrentWindow = win;
            s_windowGroupCount = 0;
        }
        internal static void InternalWindowEnd()
        {
            RigelUtility.Assert(s_windowGroupCount == 0);
        }


        public static void BeginGroup(Vector4 rect)
        {
            if(s_groupStack.Count == 0)
            {
                s_groupStack.Push(rect);
            }
            else
            {
                Vector4 root = s_groupStack.Peek();
                rect.X = MathUtil.Clamp(rect.X, 0, root.Z);
                rect.Y = MathUtil.Clamp(rect.Y, 0, root.W);

                rect.Z = MathUtil.Clamp(rect.Z, 0, root.Z - rect.X);
                rect.W = MathUtil.Clamp(rect.W, 0, root.W - rect.Y);

                rect.X += root.X;
                rect.Y += root.Y;

                s_groupStack.Push(rect);
            }

            s_windowGroupCount++;
        }

        public static void EndGroup()
        {
            RigelUtility.Assert(s_groupStack.Count > 0);
            s_groupStack.Pop();
            s_windowGroupCount--;
        }


        public static void DrawRect(Vector4 rect,Vector4 color)
        {
            InternalContext.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            InternalContext.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            InternalContext.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X +rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            InternalContext.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });

            s_depthz -= s_depthStep;
        }

        public static void DrawTextDebug(Vector4 rect,Vector4 color)
        {
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(0f,0f)
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(0,1)
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,1)
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,0)
            });

            s_depthz -= s_depthStep;
        }
        public static void DrawText(Vector4 rect,string content,Vector4 color)
        {
            rect.X += 3;
            rect.Y += 3;
            var w = 0;
            foreach(var c in content)
            {
                int xoff = DrawChar(rect, c, color);
                w += xoff;
                rect.X += xoff;
                if (w > rect.Z) break;
            }
        }
        public static int DrawChar(Vector4 rect,uint c,Vector4 color)
        {
            var glyph = InternalContext.Font.GetGlyphInfo(c);
            if (glyph == null) return (int)InternalContext.Font.FontPixelSize;

            float x1 = rect.X + glyph.LineOffsetX;
            float y1 = rect.Y + glyph.LineOffsetY;
            float x2 = x1 + glyph.PixelWidth;
            float y2 = y1 + glyph.PixelHeight;

            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[0]
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[1]
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[2]
            });
            InternalContext.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2,y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[3]
            });

            s_depthz -= s_depthStep;

            return glyph.AdvancedX;
        }




        #region utility

        internal static bool RectContainsCheck(Vector2 pos,Vector2 size,Vector2 point)
        {
            if (point.X < pos.X || point.X > pos.X + size.X) return false;
            if (point.Y < pos.Y || point.Y > pos.Y + size.Y) return false;
            return true;
        }


        #endregion
    }


    

    


}
