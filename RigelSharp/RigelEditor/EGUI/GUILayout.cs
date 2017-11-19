using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

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

        public static GUILayoutInfo CurrentLayout { get { return s_ctx.currentLayout; } }


        private static Stack<GUILayoutInfo> layoutStack { get { return s_ctx.layoutStack; } }

        public static GUIStackValue<int> s_svLineHeight = new GUIStackValue<int>(23);
        public static GUIStackValue<int> s_svLineIndent = new GUIStackValue<int>(5);
        public static int layoutOffX = 1;
        public static int layoutOffY = 1;

        public static Vector2 SizeRemain
        {
            get
            {
                return s_ctx.currentArea.Size() - s_ctx.currentLayout.Offset;
            }
        }

        /// <summary>
        /// Begin Group and Area
        /// </summary>
        /// <param name="rect">related rect</param>
        public static void BeginContainer(Vector4 rect, Vector4? color = null, params GUIOption[] options)
        {
            var rectab = GetRectAbsolute(rect);
            GUILayout.BeginArea(rectab, color, options);
            GUI.BeginGroup(rectab, null, true);
        }

        public static void EndContainer()
        {
            GUI.EndGroup();
            GUILayout.EndArea();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="options">GUIOptionType.Border</param>
        public static void BeginArea(Vector4 rect, Vector4? color = null, params GUIOption[] options)
        {
            if (color != null)
            {
                GUI.DrawRect(rect, (Vector4)color, true);
            }

            if (options != null)
            {
                var optborder = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.border; });
                if (optborder != null)
                {
                    GUI.DrawBorder(rect.Padding(-1), 1, optborder.Vector4Value, true);
                }
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
        public static void Space(float height)
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

        internal static void AutoCaculateOffset(float w, float h)
        {
            AutoCaculateOffset((int)w, (int)h);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options">GUIOptionType.Width GUIOptionType.expended GUIOptionType.Grid</param>
        /// <returns></returns>
        public static bool Button(string label, params GUIOption[] options)
        {
            return Button(label, GUI.Context.BackgroundColor.Value, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="color"></param>
        /// <param name="options">GUIOptionType.Width GUIOptionType.expended GUIOptionType.Grid</param>
        /// <returns></returns>
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
                    else if (opt.type == GUIOption.GUIOptionType.expended)
                    {
                        width = (int)(s_ctx.currentArea.Z - s_ctx.currentLayout.Offset.X);
                        adaptive = false;
                    }
                    else if (opt.type == GUIOption.GUIOptionType.grid)
                    {
                        width = (int)(s_ctx.currentArea.Z * opt.FloatValue);
                        adaptive = false;
                    }
                }
            }

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, width, s_svLineHeight.Value);
            bool valid = GUIUtility.RectClip(ref rect, curarea);


            bool clicked = false;
            if (valid)
            {
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
            }
            AutoCaculateOffsetW(width);

            return clicked;
        }

        public static void Text(string content,Vector4? color = null,params GUIOption[] options)
        {
            var optwidth = options != null ? options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.width; }) : null;
            bool adaptive = optwidth == null;

            var curarea = s_ctx.currentArea;
            var rect = new Vector4(s_ctx.currentLayout.Offset, adaptive ? curarea.Z : (int)optwidth.value, s_svLineHeight.Value);
            bool valid = GUIUtility.RectClip(ref rect, curarea);
            int width = 0;
            if (valid)
            {
                if (adaptive)
                {
                    width = GUI.DrawText(rect, content, color ??s_ctx.Color, true, GUIOption.Adaptive);
                    AutoCaculateOffsetW(width);
                }
                else
                {
                    width = GUI.DrawText(rect, content, s_ctx.Color, true, options);
                    AutoCaculateOffsetW((int)rect.Z);
                }
            }
        }


        public static void TextBlock(string content, params GUIOption[] options)
        {
            GUIOption optwidth = null;
            GUIOption optheight = null;

            if (options != null)
            {
                foreach (var o in options)
                {
                    if (o.type == GUIOption.GUIOptionType.width) optwidth = o;
                    if (o.type == GUIOption.GUIOptionType.height) optheight = o;
                }
            }
            var remain = SizeRemain;
            if (optwidth != null) remain.X = optwidth.IntValue;
            if (optheight != null) remain.Y = optheight.IntValue;

            var drawRect = new Vector4(s_ctx.currentLayout.Offset, remain.X, remain.Y);
            bool valid = GUIUtility.RectClip(ref drawRect, s_ctx.currentArea);

            int drawheight = 0;
            if (valid)
            {
                drawheight = GUI.TextBlock(drawRect, content, GUI.Context.Color, true);
            }
            AutoCaculateOffset(drawRect.Z, drawheight);


        }

        public static void Line(int thickness, Vector4? color, int margin = 2)
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

        public static bool BeginCollapseGroup(string label, bool open)
        {
            if (Button(label, GUIOption.Expended))
            {
                open = !open;
            }
            return open;
        }
        public static bool BeginCollapseGroup(string label, ref bool open)
        {
            GUILayout.Space(1);
            open = BeginCollapseGroup(label, open);
            return open;
        }
        public static void EndCollapseGroup()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="scrolltype"></param>
        /// <param name="options">GUIOption.With GUIOption.Height</param>
        /// <returns></returns>
        public static Vector2 BeginScrollView(Vector2 pos, GUIScrollType scrolltype = GUIScrollType.Vertical, params GUIOption[] options)
        {
            GUIOption optWidth = null;
            GUIOption optHeight = null;
            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt.type == GUIOption.GUIOptionType.width) { optWidth = opt; continue; }
                    if (opt.type == GUIOption.GUIOptionType.height) { optHeight = opt; continue; }
                }
            }

            var sizeremain = SizeRemain;
            if (optWidth != null) sizeremain.X = optWidth.IntValue;
            if (optHeight != null) sizeremain.Y = optHeight.IntValue;

            var rect = new Vector4(s_ctx.currentLayout.Offset, sizeremain.X, sizeremain.Y);
            var rectab = GetRectAbsolute(rect);
            var scrollView = GUI.GetScrollView(rectab);
            BeginArea(rectab, null, GUIOption.Border());
            return scrollView.Draw(rectab, pos, scrolltype);
        }
        public static void EndScrollView()
        {
            var rect = s_ctx.currentArea;
            var sv = GUI.GetScrollView(rect);
            sv.LateDraw();

            EndArea();

            GUILayout.AutoCaculateOffset(rect.Z, rect.W);
        }

        public static void SetLineHeight(int height)
        {
            s_svLineHeight.Set(height);
        }
        public static void RestoreLineHeight()
        {
            s_svLineHeight.Restore();
        }

        public static void DrawMenuList(GUIMenuList menulist, params GUIOption[] options)
        {
            var pos = s_ctx.GetNextDrawPos();
            if (Button(menulist.Label, options))
            {
                pos.Y += s_ctx.currentLayout.LastDrawHeight + 1;
                menulist.InternalSetStartPos(pos);
                GUI.DrawComponent(menulist);
            }
        }

        public static void DrawRect(Vector4 rect, Vector4 color,params GUIOption[] options)
        {
            GUIUtility.RectClip(ref rect, s_ctx.currentArea);
            GUI.DrawRect(rect, color, true);
        }

        public static void DrawRectOnFlow(Vector2 size, Vector4 color,params GUIOption[] options)
        {
            var offset = s_ctx.currentLayout.Offset;
            Vector4 rect = new Vector4(offset, size.X,size.Y);
            GUIUtility.RectClip(ref rect, s_ctx.currentArea);
            if (options != null)
            {
                var optcheckcontains = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.checkRectContains; });
                if (optcheckcontains != null) {
                    optcheckcontains.value = GUIUtility.RectContainsCheck(rect, GUI.Event.Pointer);
                }
            }
            GUI.DrawRect(rect, color, true,GUIOption.NoClip);
        }


        #region 

        public static string TextInput(string content,params GUIOption[] options)
        {
            return content;
        }

        public static string TextInput(string label,string content,params GUIOption[] options)
        {
            var sizer = GUILayout.SizeRemain;
            var rect = new Vector4(s_ctx.currentLayout.Offset, sizer.X, s_svLineHeight);
            var rectAbsolute = GetRectAbsolute(rect);

            GUIObjTextInput input = GUI.GetTextInput(rectAbsolute);
            input.Draw(rect, content,label);

            Space(1);

            return content;
        }



        #endregion

        #region Utility
        public static Vector4 GetRectAbsolute(Vector4 rect)
        {
            var off = s_ctx.currentArea.Pos();
            rect.X += off.X;
            rect.Y += off.Y;
            return rect;
        }
        #endregion
    }
}
