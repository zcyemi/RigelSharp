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
        internal static RigelEGUICtx s_currentCtx = null;
        internal static RigelEGUIWindow s_currentWindow = null;
        internal static float s_depthz = 1;
        internal static float s_depthStep = 0.0001f;
        internal static RigelEGUIEvent m_event = null;
        public static RigelEGUIEvent Event { get { return m_event; } }


        public static void DrawRect(Vector4 rect,Vector4 color)
        {
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X +rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });

            s_depthz -= s_depthStep;
        }

        public static void DrawTextDebug(Vector4 rect,Vector4 color)
        {
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(0f,0f)
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(0,1)
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y + rect.W, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,1)
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, s_depthz, 1),
                Color = color,
                UV = new Vector2(1,0)
            });

            s_depthz -= s_depthStep;
        }

        public static void DrawText(Vector4 rect,string content,Vector4 color)
        {
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
            var glyph = s_currentCtx.Font.GetGlyphInfo(c);
            if (glyph == null) return (int)s_currentCtx.Font.FontPixelSize;

            float x1 = rect.X + glyph.LineOffsetX;
            float y1 = rect.Y + glyph.LineOffsetY;
            float x2 = x1 + glyph.PixelWidth;
            float y2 = y1 + glyph.PixelHeight;

            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[0]
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[1]
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2,y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[2]
            });
            s_currentCtx.BufferText.Add(new RigelEGUIVertex()
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
