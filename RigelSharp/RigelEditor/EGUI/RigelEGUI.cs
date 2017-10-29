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
        private static Vector4 s_groupCurrent;
        private static int s_windowGroupCount = 0;

        private static List<RigelEGUIVertex> BufferText;
        private static List<RigelEGUIVertex> BufferRect;

        internal static void InternalResetContext(RigelEGUICtx ctx)
        {
            InternalContext = ctx;
            s_groupStack.Clear();
        }
        internal static void InternalFrameBegin(RigelEGUIEvent e)
        {
            Event = e;
            s_groupStack.Clear();
            s_groupCurrent = new Vector4(0, 0, InternalContext.ClientWidth, InternalContext.ClientHeight);
            s_groupStack.Push(s_groupCurrent);

            SetDrawTarget(true);
        }
        internal static void InternalFrameEnd()
        {

        }
        
        internal static void SetDrawTarget(bool main)
        {
            if (main)
            {
                BufferText = InternalContext.BufMainText;
                BufferRect = InternalContext.BufMainRect;
            }
            else
            {
                BufferText = InternalContext.BufferText;
                BufferRect = InternalContext.BufferRect;
            }
        }

        internal static void InternalWindowBegin(RigelEGUIWindow win)
        {
            SetDrawTarget(false);

            CurrentWindow = win;
            s_windowGroupCount = 0;
            RigelEGUI.BeginGroup(new Vector4(win.Position, win.Size.X, win.Size.Y));

            RigelEGUILayout.s_layoutOffset = Vector2.Zero;
        }
        internal static void InternalWindowEnd()
        {
            RigelEGUI.EndGroup();   //end content group
            RigelEGUI.EndGroup();   //end window group
            RigelUtility.Assert(s_windowGroupCount == 0);

            SetDrawTarget(true);
        }


        public static void BeginGroup(Vector4 rect)
        {
            if(s_groupStack.Count == 0)
            {
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
            }

            s_groupCurrent = rect;
            s_groupStack.Push(rect);

            s_windowGroupCount++;
        }

        public static void EndGroup()
        {
            RigelUtility.Assert(s_groupStack.Count > 0);
            s_groupStack.Pop();
            s_groupCurrent = s_groupStack.Peek();
            s_windowGroupCount--;
        }

        public static void DrawRectRaw(Vector4 rect, Vector4 color)
        {
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });

            s_depthz -= s_depthStep;
        }

        public static bool DrawRect(Vector4 rect,Vector4 color)
        {
            bool valid = RectClip(ref rect, s_groupCurrent);

            if (!valid) return false;

            DrawRectRaw(rect, color);
            return true;
        }
        public static void DrawTextDebug(Vector4 rect,Vector4 color)
        {
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(0f,0f)
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(0,1)
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,1)
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,0)
            });

            s_depthz -= s_depthStep;
        }
        public static void DrawText(Vector4 rect,string content,Vector4 color)
        {
            RigelEGUI.RectClip(ref rect, s_groupCurrent);
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

            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[0]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[1]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[2]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2,y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[3]
            });

            s_depthz -= s_depthStep;

            return glyph.AdvancedX;
        }


        public static bool Button(Vector4 rect,string label,Vector4 col,Vector4 texcolor)
        {
            return Button(rect.Pos(), rect.Size(), label, col, texcolor);
        }
        public static bool Button(Vector2 pos,Vector2 size,string label,Vector4 color,Vector4 texcolor)
        {
            Vector4 rect = new Vector4(pos.X, pos.Y, size.X, size.Y);
            DrawRect(rect, color);
            DrawText(rect, label, texcolor);

            if (Event.Used) return false;
            if (Event.EventType != RigelEGUIEventType.MouseClick) return false;

            var cpos = pos + s_groupCurrent.Pos();
            if (RectContainsCheck(cpos, size, Event.Pointer))
            {
                Event.Use();
                return true;
            }
            return false;
        }


        #region utility

        internal static bool RectContainsCheck(Vector2 pos,Vector2 size,Vector2 point)
        {
            if (point.X < pos.X || point.X > pos.X + size.X) return false;
            if (point.Y < pos.Y || point.Y > pos.Y + size.Y) return false;
            return true;
        }
        /// <summary>
        /// Clicp Rect With Group
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="group"></param>
        /// <returns>return false if the size of clipped rect is zero</returns>
        internal static bool RectClip(ref Vector4 rect,Vector4 group)
        {
            rect.X = MathUtil.Clamp(rect.X, 0, group.Z);
            rect.Y = MathUtil.Clamp(rect.Y, 0, group.W);

            rect.Z = MathUtil.Clamp(rect.Z, 0, group.Z - rect.X);
            rect.W = MathUtil.Clamp(rect.W, 0, group.W - rect.Y);

            rect.X += group.X;
            rect.Y += group.Y;

            if (rect.Z < 1.0f || rect.W < 1.0f) return false;
            return true;
        }

        #endregion
    }


    

    


}
