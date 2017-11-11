using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public struct GUIGroupInfo
    {
        public Vector4 Rect;
        public Vector4 Absolute;
    }

    public class GUICtx
    {
        public Vector4 color = RigelColor.White;
        public Vector4 backgroundColor = RigelColor.RGBA(128, 128, 128, 255);

        /// <summary>
        /// groupStack contains currentGroup
        /// </summary>
        //public Stack<Vector4> groupStack = new Stack<Vector4>();
        public Stack<GUIGroupInfo> groupStack = new Stack<GUIGroupInfo>();
        public GUIGroupInfo currentGroup;
        public Vector4 baseRect;
        ///// <summary>
        ///// relative to parent group
        ///// </summary>
        //public Vector4 currentGroup;
        ///// <summary>
        ///// relative to baseRect
        ///// </summary>
        //public Vector4 currentGroupAbsolute;

        public Stack<Vector4> areaStack = new Stack<Vector4>();
        public Stack<GUILayoutInfo> layoutStack = new Stack<GUILayoutInfo>();
        /// <summary>
        /// relative to baseRect
        /// </summary>
        public Vector4 currentArea;
        public GUILayoutInfo currentLayout;
        public Vector4 GetNextDrawPos()
        {
            var rect = currentArea;
            rect.X += currentLayout.Offset.X;
            rect.Y += currentLayout.Offset.Y;

            return rect;
        }




        public float s_depthStep = 0.0001f;

        public RigelFont Font { get; set; }

        //component
        public Stack<IGUIComponent> componentStack = new Stack<IGUIComponent>();


        public void Frame(RigelEGUIEvent guievent, int width, int height)
        {
            RigelUtility.Assert(groupStack.Count == 0);
            RigelUtility.Assert(areaStack.Count == 0);
            RigelUtility.Assert(layoutStack.Count == 0);

            GUI.Event = guievent;
            baseRect = new Vector4(0, 0, width, height);
            currentGroup.Rect = baseRect;
            currentGroup.Absolute = baseRect;

            //layout
            currentLayout.Offset = Vector2.Zero;
            currentLayout.SizeMax = Vector2.Zero;
            currentLayout.Verticle = true;

            currentArea = baseRect;
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

    public struct GUILayoutInfo
    {
        public bool Verticle;
        public Vector2 Offset;
        public Vector2 SizeMax;

        public int LastDrawHeight;
        public int LastDrawWidth;
    }


    public class GUIStackValue<T>
    {
        private Stack<T> m_stack = new Stack<T>();
        public T Value { get; private set; }


        public GUIStackValue(T defaultval)
        {
            Value = defaultval;
            m_stack.Push(Value);
        }

        public void Set(T v)
        {
            m_stack.Push(Value);
            Value = v;
        }
        public void Restore()
        {
            Value = m_stack.Pop();
        }
    }

    public static class GUI
    {
        public static GUICtx s_ctx;
        public static GUICtx Context
        {
            get { return s_ctx; }
            set { s_ctx = value; GUILayout.s_ctx = s_ctx; }
        }
        public static GUIDrawTarget DrawTarget { get; private set; }

        public static RigelEGUIEvent Event { get; set; }

        internal static List<RigelEGUIVertex> BufferRect { get { return DrawTarget.bufferRect; } }
        internal static List<RigelEGUIVertex> BufferText { get { return DrawTarget.bufferText; } }

        private static float s_depthz;
        public static float Depth { get { return s_depthz; } }

        public static void BeginGroup(Vector4 rect, Vector4? color = null, bool absolute = false)
        {
            var groupStack = s_ctx.groupStack;
            if (absolute)
            {
                if (color != null) DrawRect(rect, (Vector4)color, true);
                s_ctx.currentGroup.Absolute = rect;
                rect.X -= s_ctx.currentGroup.Rect.X;
                rect.Y -= s_ctx.currentGroup.Rect.Y;
                s_ctx.currentGroup.Rect = rect;

                groupStack.Push(s_ctx.currentGroup);
            }
            else
            {
                Vector4 root = s_ctx.currentGroup.Rect;

                rect.X = MathUtil.Clamp(rect.X, 0, root.Z);
                rect.Y = MathUtil.Clamp(rect.Y, 0, root.W);

                rect.Z = MathUtil.Clamp(rect.Z, 0, root.Z - rect.X);
                rect.W = MathUtil.Clamp(rect.W, 0, root.W - rect.Y);

                if (color != null) DrawRect(rect, (Vector4)color);
                s_ctx.currentGroup.Rect = rect;

                var groupab = s_ctx.currentGroup.Absolute;
                groupab.X += rect.X;
                groupab.Y += rect.Y;
                groupab.Z = rect.Z;
                groupab.W = rect.W;
                s_ctx.currentGroup.Absolute = groupab;

                groupStack.Push(s_ctx.currentGroup);
            }
        }

        public static void EndGroup(bool absolute = false)
        {
            var groupStack = s_ctx.groupStack;
            RigelUtility.Assert(groupStack.Count > 0);
            var curGroup = groupStack.Pop();

            if (groupStack.Count == 0)
            {
                s_ctx.currentGroup.Rect = s_ctx.baseRect;
                s_ctx.currentGroup.Absolute = s_ctx.baseRect;
            }
            else
            {
                s_ctx.currentGroup = groupStack.Peek();
            }


            //if (absolute)
            //{
            //    Vector4 ab = Vector4.Zero;
            //    foreach(var g in groupStack)
            //    {
            //        ab.X += g.X;
            //        ab.Y += g.Y;
            //    }

            //    ab.Z = s_ctx.currentGroup.Z;
            //    ab.W = s_ctx.currentGroup.W;

            //    s_ctx.currentGroupAbsolute = ab;
            //}
            //else
            //{
            //    var groupab = s_ctx.currentGroupAbsolute;
            //    groupab.X -= curGroup.X;
            //    groupab.Y -= curGroup.Y;
            //    groupab.Z = s_ctx.currentGroup.Z;
            //    groupab.W = s_ctx.currentGroup.W;

            //    s_ctx.currentGroupAbsolute = groupab;
            //}


        }

        public static bool Button(Vector4 rect, string label, bool absolute = false, params GUIOption[] options)
        {
            return Button(rect, label, Context.color, Context.backgroundColor, absolute, options);
        }

        public static bool Button(Vector4 rect, string label, Vector4 color, Vector4 texcolor, bool absolute = false, params GUIOption[] options)
        {
            if (!absolute)
            {
                var off = s_ctx.currentGroup.Rect.Pos();
                rect.X += off.X;
                rect.Y += off.Y;
            }

            bool clicked = false;
            if (GUIUtility.RectContainsCheck(rect, Event.Pointer))
            {
                if (options != null)
                {
                    var optCheckRC = options.FirstOrDefault((o) => { return o.type == GUIOption.GUIOptionType.checkRectContains; });
                    if (optCheckRC != null) optCheckRC.value = true;
                }


                if (!Event.Used && Event.EventType == RigelEGUIEventType.MouseClick)
                {
                    Event.Use();
                    clicked = true;

                }
                if (Event.Button == MouseButton.Left)
                {
                    DrawRect(rect, GUIStyle.Current.ColorActiveD, absolute);
                }
                else
                {
                    DrawRect(rect, GUIStyle.Current.ColorActive, absolute);
                }
            }
            else
            {
                DrawRect(rect, color, absolute);
            }
            DrawText(rect, label, texcolor, absolute);

            return clicked;
        }

        public static void Label(Vector4 position, string text, bool absolute = false)
        {
            DrawText(position, text, Context.color, absolute);
        }

        public static int DrawText(Vector4 rect, string content, Vector4 color, bool absolute = false)
        {
            int pixelsize = (int)Context.Font.FontPixelSize;
            rect.Y += rect.W > pixelsize ? (rect.W - pixelsize) / 2 : 0;

            GUIUtility.RectClip(ref rect, absolute ? s_ctx.baseRect : s_ctx.currentGroup.Absolute);
            int w = 0;

            //centered
            int textwidth = Context.Font.GetTextWidth(content);
            w = rect.Z > textwidth ? (int)((rect.Z - textwidth) / 2) : 1;
            rect.X += w;


            foreach (var c in content)
            {
                int xoff = DrawChar(rect, c, color);
                w += xoff;
                rect.X += xoff;
                if (w > rect.Z) break;
            }

            return w;
        }

        public static int DrawChar(Vector4 rect, uint c, Vector4 color)
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

        public static bool Toggle(Vector4 rect, bool value, string label)
        {
            return value;
        }

        public static void DrawRect(Vector4 rect, bool absolute = false)
        {
            DrawRect(rect, Context.backgroundColor, absolute);
        }

        public static void DrawRect(Vector4 rect, Vector4 color, bool absolute = false)
        {
            var valid = GUIUtility.RectClip(ref rect, absolute ? s_ctx.baseRect : s_ctx.currentGroup.Absolute);
            if (!valid) return;

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

        public static void DrawTexture(Vector4 rect, uint textureid)
        {

        }

        public static string TextField(Vector4 rect, string content)
        {
            return null;
        }

        public static Vector2 BeginScrollView(Vector4 rect, Vector2 pos)
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


        public static void DrawComponent(IGUIComponent component)
        {
            component.InitDrawed = false;
            component.Distroy = false;
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

        internal static void SetDepth(float depth)
        {
            s_depthz = depth;
        }

        public static Vector4 GetRectAbsolute(Vector4 rect)
        {
            var off = Context.currentGroup.Absolute.Pos();
            rect.X += off.X;
            rect.Y += off.Y;

            return rect;
        }

        #endregion
    }

    public static class GUILayout
    {
        //reference to GUI.s_ctx
        public static GUICtx s_ctx;


        private static Stack<GUILayoutInfo> layoutStack { get { return s_ctx.layoutStack; } }

        public static GUIStackValue<int> s_svLineHeight = new GUIStackValue<int>(25);
        public static GUIStackValue<int> s_svLineIndent = new GUIStackValue<int>(5);
        public static int layoutOffX = 1;
        public static int layoutOffY = 1;


        /// <summary>
        /// area always absolute to the baseRect
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public static void BeginArea(Vector4 rect, Vector4? color = null)
        {
            if (color != null)
            {
                GUI.DrawRect(rect, (Vector4)color, true);
            }
            s_ctx.areaStack.Push(s_ctx.currentArea);
            s_ctx.currentArea = rect;

            var curlayout = s_ctx.currentLayout;
            layoutStack.Push(curlayout);

            curlayout.Verticle = true;
            curlayout.SizeMax = Vector2.Zero;
            curlayout.Offset = Vector2.Zero;

            s_ctx.currentLayout = curlayout;
        }

        public static void EndArea()
        {
            s_ctx.currentArea = s_ctx.areaStack.Pop();
            s_ctx.currentLayout = s_ctx.layoutStack.Pop();
        }

        public static void BeginHorizontal()
        {

            s_ctx.currentLayout.Verticle = false;
            s_ctx.layoutStack.Push(s_ctx.currentLayout);
            s_ctx.currentLayout.SizeMax.Y = 0;
        }

        public static void EndHorizontal()
        {
            var playout = s_ctx.layoutStack.Pop();
            if (layoutStack.Count == 0)
            {
                s_ctx.currentLayout.Verticle = true;
            }
            else
            {
                s_ctx.currentLayout.Verticle = layoutStack.Peek().Verticle;
            }


            var lastOffset = playout.Offset;

            var off = s_ctx.currentLayout.Offset - lastOffset;
            s_ctx.currentLayout.SizeMax.X = Math.Max(off.X, s_ctx.currentLayout.SizeMax.X);
            s_ctx.currentLayout.Offset.Y += s_ctx.currentLayout.SizeMax.Y > s_svLineHeight.Value ? s_ctx.currentLayout.SizeMax.Y : s_svLineHeight.Value;
            s_ctx.currentLayout.Offset.X = lastOffset.X;
        }

        public static void BeginVertical()
        {
            s_ctx.currentLayout.Verticle = true;
            layoutStack.Push(s_ctx.currentLayout);
            s_ctx.currentLayout.SizeMax.X = 0;
        }

        public static void EndVertical()
        {
            var playout = layoutStack.Pop();
            s_ctx.currentLayout.Verticle = layoutStack.Peek().Verticle;
            var lastOffset = playout.Offset;

            var off = s_ctx.currentLayout.Offset - lastOffset;

            s_ctx.currentLayout.SizeMax.Y = Math.Max(off.Y, s_ctx.currentLayout.SizeMax.Y);

            s_ctx.currentLayout.Offset.X += s_ctx.currentLayout.SizeMax.X > 5f ? s_ctx.currentLayout.SizeMax.X : 5f;
            s_ctx.currentLayout.Offset.Y = lastOffset.Y;
        }

        public static void Space()
        {
            s_ctx.currentLayout.Offset.Y += s_svLineHeight.Value;
        }
        public static void Space(int height)
        {
            s_ctx.currentLayout.Offset.Y += height;
        }

        public static void Indent()
        {
            s_ctx.currentLayout.Offset.X += s_svLineIndent.Value;
        }
        public static void Indent(int width)
        {
            s_ctx.currentLayout.Offset.X += width;
        }


        internal static void AutoCaculateOffset(int w, int h)
        {
            var layout = s_ctx.currentLayout;
            if (layout.Verticle)
            {
                layout.Offset.Y += h + layoutOffY;
                layout.SizeMax.X = Math.Max(layout.SizeMax.X, w);
            }
            else
            {
                layout.Offset.X += w + layoutOffX;
                layout.SizeMax.Y = Math.Max(layout.SizeMax.Y, h);
            }
            layout.LastDrawWidth = w;
            layout.LastDrawHeight = h;


            s_ctx.currentLayout = layout;
        }
        internal static void AutoCaculateOffsetW(int w)
        {
            AutoCaculateOffset(w, s_svLineHeight.Value);
        }
        internal static void AutoCaculateOffsetH(int h)
        {
            AutoCaculateOffset(s_svLineIndent.Value, h);
        }

        public static bool Button(string label, params GUIOption[] options)
        {
            int width = 50;
            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt.type == GUIOption.GUIOptionType.width) width = (int)opt.value;
                }
            }

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, width, s_svLineHeight.Value);
            GUIUtility.RectClip(ref rect, curarea);
            var ret = GUI.Button(rect, label, true);
            AutoCaculateOffsetW(width);

            return ret;
        }

        public static bool Button(string label, Vector4 color, params GUIOption[] options)
        {
            int width = 50;
            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt.type == GUIOption.GUIOptionType.width) width = (int)opt.value;
                }
            }

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, width, s_svLineHeight.Value);
            GUIUtility.RectClip(ref rect, curarea);
            var ret = GUI.Button(rect, label, color, GUI.Context.color, true, options);
            AutoCaculateOffsetW(width);

            return ret;
        }


        public static void Text(string content)
        {
            var rect = new Vector4(s_ctx.currentLayout.Offset, 400, s_svLineHeight.Value);
            var curarea = s_ctx.currentArea;
            GUIUtility.RectClip(ref rect, curarea);
            var width = GUI.DrawText(rect, content, s_ctx.color, true);

            AutoCaculateOffsetW(width);
        }

        public static void BeginToolBar(int height)
        {
            SetLineHeight(height);
            var rect = new Vector4(s_ctx.currentLayout.Offset, s_ctx.currentArea.Z, height);
            DrawRect(rect, GUIStyle.Current.MainMenuBGColor);
            BeginHorizontal();

        }

        public static void EndToolBar()
        {
            EndHorizontal();
            RestoreLineHeight();
        }

        public static void SetLineHeight(int height)
        {
            s_svLineHeight.Set(height);
        }
        public static void RestoreLineHeight()
        {
            s_svLineHeight.Restore();
        }

        public static void DrawMenuList(GUIMenuList menulist)
        {
            var pos = s_ctx.GetNextDrawPos();
            if (Button(menulist.Label))
            {
                pos.Y += s_ctx.currentLayout.LastDrawHeight + 1;
                menulist.InternalSetStartPos(pos);
                GUI.DrawComponent(menulist);
            }
        }

        public static void DrawRect(Vector4 rect, Vector4 color)
        {
            GUIUtility.RectClip(ref rect, s_ctx.currentArea);
            GUI.DrawRect(rect, color, true);
        }

    }

    public class GUIDragState
    {
        private Vector2 m_offset;
        private bool m_ondrag = false;
        public Vector2 OffSet { get { return m_offset; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">absolute rect</param>
        /// <returns></returns>
        public bool OnDrag(Vector4 rect)
        {
            if (GUI.Event.Used) return false;
            bool constains = GUIUtility.RectContainsCheck(rect, GUI.Event.Pointer);
            return OnDrag(constains);
        }

        public bool OnDrag(bool constainsCheck = true)
        {
            if (GUI.Event.Used) return false;

            var e = GUI.Event;
            if (e.EventType == RigelEGUIEventType.MouseDragEnter)
            {
                if (constainsCheck)
                {
                    m_ondrag = true;
                    e.Use();
                }
            }
            else if (e.EventType == RigelEGUIEventType.MouseDragLeave)
            {
                if (m_ondrag)
                {
                    e.Use();
                    m_ondrag = false;
                }
            }
            else if (e.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if (m_ondrag)
                {
                    e.Use();
                }
                m_offset = e.DragOffset;
                return true;
            }

            return false;
        }
    }
}
