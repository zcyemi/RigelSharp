using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{

    public struct GUILayoutInfo
    {
        public bool Verticle;
        public Vector2 Offset;
        public Vector2 SizeMax;

        public int LastDrawHeight;
        public int LastDrawWidth;
        public Vector2 LastOffset;
        public Vector4 LastRect;
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
            layout.LastOffset = layout.Offset;

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
            layout.LastRect = new Vector4(layout.LastOffset, w, h);


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
            return Button(label, GUI.Context.BackgroundColor.Value, options);
        }

        public static bool Button(string label, Vector4 color, params GUIOption[] options)
        {
            int width = 50;
            bool adaptive = true;
            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt.type == GUIOption.GUIOptionType.width)
                    {
                        width = (int)opt.value;
                        adaptive = false;
                    }
                }
            }

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, width, s_svLineHeight.Value);
            GUIUtility.RectClip(ref rect, curarea);

            bool clicked = false;
            if (adaptive)
            {
                var adaptiveValue = GUIOption.AdaptiveValue();
                clicked = GUI.Button(rect, label, color, GUI.Context.Color, true, options.Append(adaptiveValue));
                width = adaptiveValue.IntValue;
            }
            else
            {
                clicked = GUI.Button(rect, label, color, GUI.Context.Color, true, options);
            }

            AutoCaculateOffsetW(width);

            return clicked;
        }

        public static void Text(string content, params GUIOption[] options)
        {
            var optwidth = options != null ? options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.width; }) : null;
            bool adaptive = optwidth == null;

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, adaptive ? curarea.Z : (int)optwidth.value, s_svLineHeight.Value);
            GUIUtility.RectClip(ref rect, curarea);
            int width = 0;
            if (adaptive)
            {
                width = GUI.DrawText(rect, content, s_ctx.Color, true, GUIOption.Adaptive);
            }
            else
            {
                width = GUI.DrawText(rect, content, s_ctx.Color, true);
            }

            AutoCaculateOffsetW(width);
        }

        public static void Line(int thickness,Vector4? color, int margin = 2)
        {
            GUILayout.Space(margin);
            var endpos = new Vector2(thickness, thickness);
            var startpos = s_ctx.currentLayout.Offset;
            if (s_ctx.currentLayout.Verticle)
            {
                endpos.X = s_ctx.currentArea.Z;
            }
            else
            {
                endpos.Y = s_ctx.currentArea.W;
            }
            GUI.DrawLineAxisAligned(startpos, endpos, thickness, color);

            GUILayout.Space(margin);
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
}
