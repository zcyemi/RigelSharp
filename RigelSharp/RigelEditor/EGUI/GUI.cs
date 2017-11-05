using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUICtx
    {
        public Vector4 color = RigelColor.White;
        public Vector4 backgroundColor = RigelColor.RGBA(128, 128, 128, 255);

        public Stack<Vector4> groupStack = new Stack<Vector4>();

        public Stack<Vector4> areaStack = new Stack<Vector4>();
        public Stack<GUILayoutInfo> layoutStack = new Stack<GUILayoutInfo>();


        public float s_depthStep = 0.0001f;

        public RigelFont Font { get; set; }

        //component
        public Stack<IGUIComponent> componentStack = new Stack<IGUIComponent>();


        public void Frame(RigelEGUIEvent guievent)
        {
            RigelUtility.Assert(groupStack.Count == 0);
            RigelUtility.Assert(areaStack.Count == 0);
            RigelUtility.Assert(layoutStack.Count == 0);

            GUI.Event = guievent;
        }

    }

    public class GUIDrawTarget
    {
        public List<RigelEGUIVertex> bufferRect;
        public List<RigelEGUIVertex> bufferText;
        public float depth;

        public GUIDrawTarget(float depth)
        {
            this.depth = depth;
            bufferRect = new List<RigelEGUIVertex>();
            bufferText = new List<RigelEGUIVertex>();
        }
    }

    public class GUILayoutInfo
    {
        public bool Verticle;
        public Vector2 Offset;
        public Vector2 SizeMax;
    }


    public static class GUI
    {
        public static GUICtx s_ctx;
        public static GUICtx Context { get { return s_ctx; } set { s_ctx = value; } }
        public static GUIDrawTarget DrawTarget { get; private set; }

        public static RigelEGUIEvent Event { get; set; }

        internal static List<RigelEGUIVertex> BufferRect { get { return DrawTarget.bufferRect; } }
        internal static List<RigelEGUIVertex> BufferText { get { return DrawTarget.bufferText; } }

        private static float s_depthz;

        public static void BeginGroup(Vector4 position,bool absulate = false)
        {

        }

        public static void EndGroup()
        {

        }

        public static bool Button(Vector4 rect, string label)
        {
            return Button(rect, label, Context.color, Context.backgroundColor);
        }

        public static bool Button(Vector4 rect,string label,Vector4 color,Vector4 texcolor)
        {
            DrawRect(rect, color);
            DrawText(rect, label, texcolor);
            if (Event.Used) return false;
            if (Event.EventType != RigelEGUIEventType.MouseClick) return false;

            if (RectContainsCheck(rect, Event.Pointer))
            {
                Event.Use();
                return true;
            }
            return false;
        }

        public static void Label(Vector4 position,string text)
        {
            DrawText(position, text, Context.color);
        }

        public static int DrawText(Vector4 rect, string content, Vector4 color)
        {
            var w = 0;
            foreach (var c in content)
            {
                int xoff = DrawChar(rect, c, color);
                w += xoff;
                rect.X += xoff;
                if (w > rect.Z) break;
            }

            return w;
        }

        public static int DrawChar(Vector4 rect,uint c,Vector4 color)
        {
            var glyph = Context.Font.GetGlyphInfo(c);
            if (glyph == null) return (int)Context.Font.FontPixelSize;

            float x1 = rect.X + glyph.LineOffsetX;
            float y1 = rect.Y + glyph.LineOffsetY;
            float x2 = x1 + glyph.PixelWidth;
            float y2 = y1 + glyph.PixelHeight;

            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1, y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[0]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x1, y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[1]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2, y2, s_depthz, 1),
                Color = color,
                UV = glyph.UV[2]
            });
            BufferText.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(x2, y1, s_depthz, 1),
                Color = color,
                UV = glyph.UV[3]
            });

            DepthIncrease();

            return glyph.AdvancedX;
        }

        public static bool Toggle(Vector4 rect,bool value,string label)
        {
            return value;
        }

        public static void DrawRect(Vector4 rect)
        {
            DrawRect(rect, Context.backgroundColor);
        }

        public static void DrawRect(Vector4 rect,Vector4 color)
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

            DepthIncrease();
        }

        public static void DrawTexture(Vector4 rect,uint textureid)
        {

        }

        public static string TextField(Vector4 rect,string content)
        {
            return null;
        }

        public static Vector2 BeginScrollView(Vector4 rect,Vector2 pos)
        {
            return pos;
        }

        public static void EndScrollView()
        {

        }

        public static void BeginWindow()
        {

        }

        public static void EndWindow()
        {

        }

        public static void BeginToolBar(Vector4 rect)
        {

        }

        public static void EndToolBar()
        {

        }

        public static void DrawComponent(IGUIComponent component)
        {
            Context.componentStack.Push(component);
            Event.Use();
        }

        #region utility
        internal static void SetDrawTarget(GUIDrawTarget target)
        {
            DrawTarget = target;
            s_depthz = target.depth;
        }
        internal static void DepthIncrease()
        {
            s_depthz -= Context.s_depthStep;
        }

        internal static bool RectClip(ref Vector4 rect, Vector4 group)
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

        internal static bool RectContainsCheck(Vector2 pos, Vector2 size, Vector2 point)
        {
            if (point.X < pos.X || point.X > pos.X + size.X) return false;
            if (point.Y < pos.Y || point.Y > pos.Y + size.Y) return false;
            return true;
        }

        internal static bool RectContainsCheck(Vector4 rect, Vector2 point)
        {
            if (point.X < rect.X || point.X > rect.X + rect.Z) return false;
            if (point.Y < rect.Y || point.Y > rect.Y + rect.W) return false;
            return true;
        }
        #endregion
    }

    public static class GUILayout
    {

    }
}
