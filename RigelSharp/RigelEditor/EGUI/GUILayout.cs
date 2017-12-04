using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;
using RigelCore.Rendering;

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


    public static partial class GUILayout
    {
        //reference to GUI.s_ctx
        public static GUICtx s_ctx;
        public static GUICtx Context { get { return s_ctx; } }


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
                return s_ctx.currentArea.Rect.Size() - s_ctx.currentLayout.Offset;
            }
        }

        /// <summary>
        /// Begin Group and Area
        /// </summary>
        /// <param name="rect">rect</param>
        public static void BeginContainer(Vector4 rect, Vector4? color = null, params GUIOption[] options)
        {
            GUILayout.BeginArea(rect, color, options);
            GUI.BeginGroup(rect, null, true);
        }

        /// <summary>
        /// relative rect
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="options"></param>
        public static void BeginContainerRelative(Vector4 rect,Vector4? color = null,params GUIOption[] options)
        {
            var rectab = GetRectAbsolute(rect);
            GUILayout.BeginAreaRelative(rect,color,options);
            GUI.BeginGroup(rectab,null, true);
        }

        public static void EndContainer()
        {
            GUI.EndGroup();
            GUILayout.EndArea();
        }


        public static void BeginAreaR(Vector4 rect,Vector4? color ,params GUIOption[] options)
        {
            rect = GetRectAbsolute(rect);
            BeginArea(rect, color, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"> absolute rect</param>
        /// <param name="color"></param>
        /// <param name="options">GUIOptionType.Border</param>
        public static void BeginArea(Vector4 recta, Vector4? color = null, params GUIOption[] options)
        {
            if (color != null)
            {
                GUI.RectA(recta, (Vector4)color);
            }

            if (options != null)
            {
                var optborder = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.border; });
                if (optborder != null)
                {
                    GUI.DrawBorder(recta, 1, optborder.Vector4Value, true);
                }
            }

            s_ctx.areaStack.Push(s_ctx.currentArea);
            s_ctx.currentArea.Rect = recta;
            s_ctx.currentArea.ContentMax = recta.Size();

            var curlayout = s_ctx.currentLayout;
            layoutStack.Push(curlayout);

            curlayout.Verticle = true;
            curlayout.SizeMax = Vector2.Zero;
            curlayout.Offset = Vector2.Zero;

            s_ctx.currentLayout = curlayout;
        }

        public static void BeginAreaRelative(Vector4 rect, Vector4? color = null, params GUIOption[] options)
        {
            rect = GetRectAbsolute(rect);
            BeginArea(rect, color, options);
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
        public static void Indent(float width)
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

            if (layout.Offset.Y > s_ctx.currentArea.ContentMax.Y)
            {
                s_ctx.currentArea.ContentMax.Y = layout.Offset.Y;
            }

            if (layout.Offset.X > s_ctx.currentArea.ContentMax.X)
            {
                s_ctx.currentArea.ContentMax.X = layout.Offset.X;
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
        internal static void AutoCaculateOffsetW(float w)
        {
            AutoCaculateOffset(w, s_svLineHeight.Value);
        }
        internal static void AutoCaculateOffsetH(float h)
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
        /// <param name="options">
        /// GUIOptionType.Width 
        /// GUIOptionType.expended 
        /// GUIOptionType.Grid
        /// </param>
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
                        width = (int)(s_ctx.currentArea.Rect.Z - s_ctx.currentLayout.Offset.X);
                        adaptive = false;
                    }
                    else if (opt.type == GUIOption.GUIOptionType.grid)
                    {
                        width = (int)(s_ctx.currentArea.Rect.Z * opt.FloatValue);
                        adaptive = false;
                    }
                }
            }

            var curarea = s_ctx.currentArea.Rect;
            var rect = new Vector4(s_ctx.currentLayout.Offset, width, s_svLineHeight.Value);

            var rectpre = rect;

            Vector4 clip;
            bool valid = GUIUtility.RectClip(ref rect, curarea,out clip);

            bool clicked = false;
            if (valid)
            {
                if (adaptive)
                {
                    var adaptiveValue = GUIOption.AdaptiveValue(curarea.X + curarea.Z - rect.X);
                    clicked = GUI.Button(rect, label, color, GUI.Context.Color, true, options.Append(adaptiveValue,GUIOption.TextClip(clip)));
                    width = adaptiveValue.IntValue;
                }
                else
                {
                    clicked = GUI.Button(rect, label, color, GUI.Context.Color, true, options.Append(GUIOption.TextClip(clip)));
                }
            }
            AutoCaculateOffsetW(width);

            return clicked;
        }

        #region Text
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        /// <param name="options">
        /// GUIOption.AlignH
        /// GUIOption.AlignV
        /// GUIOption.Width
        /// GUIOption.Border
        /// </param>
        /// <returns></returns>
        public static int Text(string content,Vector4? color,Vector4? bgcolor,int padding = 3,params GUIOption[] options)
        {
            Vector2 pos = Vector2.Zero;
            pos.X = padding;
            


            int textWidth = Context.Font.GetTextWidth(content);
            int textWidthN = textWidth + padding * 2;
            var rect = new Vector4(s_ctx.currentLayout.Offset, textWidthN, s_svLineHeight.Value);


            bool adaptiveW = true;
            if(options != null)
            {
                var optWidth = options.FirstOrDefault((o) => { return o.type == GUIOption.GUIOptionType.width; });
                if(optWidth != null)
                {
                    rect.Z = optWidth.IntValue;
                    adaptiveW = true;
                }
            }

            var rectcliped = rect;
            bool valid = GUIUtility.RectClip(ref rectcliped, Context.currentArea.Rect);
            if (!valid)
            {
                AutoCaculateOffset(textWidthN, s_svLineHeight.Value + 1);
                return textWidthN;
            }


            pos.Y = (int)((rect.W - Context.Font.FontPixelSize - padding) / 2);
            if (pos.Y < 0) pos.Y = padding;

            GUIOption optBorder = null;

            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt == GUIOption.AlignHRight)
                    {
                        pos.X = (int)(rect.Z - textWidth- padding);
                        if (pos.X < 0) pos.X = padding;
                        adaptiveW = false;
                        continue;
                    }else if(opt == GUIOption.AlignHLeft)
                    {
                        pos.X = padding;
                        adaptiveW = false;
                        continue;
                    }
                    if(opt == GUIOption.AlignVTop)
                    {
                        pos.Y = padding;   
                        continue;
                    }
                    else if(opt == GUIOption.AlignVBottom)
                    {
                        pos.Y = rect.W - Context.Font.FontPixelSize - padding;
                        if(pos.Y < 0) pos.Y = padding;
                        continue;
                    }

                    if(opt.type == GUIOption.GUIOptionType.border)
                    {
                        optBorder = opt;
                    }
                }
            }

            if (adaptiveW)
            {
                pos.X = (int)((rect.Z - textWidth) / 2);
                if (pos.X < 0) pos.X = padding;
            }

            if(bgcolor != null)
            {
                GUI.RectA(rectcliped, (Vector4)bgcolor);
            }

            GUI._ImplDrawTextA(rectcliped, pos, content, color?? Context.Color);

            if(optBorder != null)
                GUI.DrawBorder(rectcliped, 1, optBorder.Vector4Value, true);

            var areaRect = Context.currentArea.Rect;

            if(rectcliped.X +rectcliped.Z < areaRect.X + areaRect.Z)
            {
                AutoCaculateOffset(rectcliped.Z,rectcliped.W +1);
            }
            else
            {
                AutoCaculateOffset(textWidthN, s_svLineHeight.Value + 1);
            }
            
            return textWidthN;
        }
        public static void Text(string content,Vector4? color,params GUIOption[] options)
        {
            Text(content, color, null, 3, options);
        }
        public static int Text(string content, params GUIOption[] options)
        {
            return Text(content, null, null, 3, options);
        }
        public static int Text(string content, Vector4 color, params GUIOption[] options)
        {
            return Text(content, color, null, 3, options);
        }
        public static int Text( string content, Vector4 color, Vector4 bgcolor, params GUIOption[] options)
        {
            return Text(content, color, bgcolor, 3, options);
        }
        public static int Text(string content, int padding, params GUIOption[] options)
        {
            return Text(content, null, null, padding, options);
        }
        #endregion



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
            bool valid = GUIUtility.RectClip(ref drawRect, s_ctx.currentArea.Rect);

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
                endpos.X = s_ctx.currentArea.Rect.Z;
            }
            else
            {
                endpos.Y = s_ctx.currentArea.Rect.W;
            }
            GUI.DrawLineAxisAligned(startpos, endpos, thickness, color);

            GUILayout.Space(margin);
        }

        public static void BeginToolBar(int height)
        {
            SetLineHeight(height);
            var rect = new Vector4(s_ctx.currentLayout.Offset, s_ctx.currentArea.Rect.Z, height);
            Rect(rect, GUIStyle.Current.MainMenuBGColor);
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
            BeginContainer(rectab,null,GUIOption.Border(GUIStyle.Current.BackgroundColorS));

            return scrollView.Draw(rectab, pos, scrolltype);
        }
        public static void EndScrollView()
        {
            var rect = s_ctx.currentArea.Rect;
            var sv = GUI.GetScrollView(rect);
            sv.LateDraw();

            EndContainer();

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


        public static void Rect(Vector4 rect,Vector4 color)
        {
            GUIUtility.RectClip(ref rect, s_ctx.currentArea.Rect);
            GUI._ImplDrawRectA(rect, color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="options">
        /// GUIOptionType.checkRectContains
        /// </param>
        public static void RectOnFlow(Vector2 size, Vector4 color,params GUIOption[] options)
        {
            var offset = s_ctx.currentLayout.Offset;
            Vector4 rect = new Vector4(offset, size.X,size.Y);
            GUIUtility.RectClip(ref rect, s_ctx.currentArea.Rect);
            if (options != null)
            {
                var optcheckcontains = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.checkRectContains; });
                if (optcheckcontains != null) {
                    optcheckcontains.value = GUIUtility.RectContainsCheck(rect, GUI.Event.Pointer);
                }
            }
            GUI._ImplDrawRectA(rect, color);
        }


        public static void DrawTexture(Vector4 rect,RenderTextureIdentifier rt)
        {
            Context.AddTextureDrawCall(rt, rect, GUI.Depth);
            GUI.DepthIncrease();
        }


        #region Widget

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
            content = input.Draw(rect, content,label);

            Space(1);

            return content;
        }


        public static int TabView(int index,List<string> tabnames,Action<int> draw,params GUIOption[] options)
        {
            var sizeRemain = GUILayout.SizeRemain;
            var rect = new Vector4(CurrentLayout.Offset, SizeRemain.X, SizeRemain.Y);
            if(options != null)
            {
                foreach(var opt in options)
                {
                    if(opt.type == GUIOption.GUIOptionType.width)
                    {
                        rect.Z = opt.IntValue;
                        continue;
                    }
                    if(opt.type == GUIOption.GUIOptionType.height)
                    {
                        rect.W = opt.IntValue;
                    }
                }
            }
            var rectab = GetRectAbsolute(rect);

            var tabview = GUI.GetTabView(rectab);

            int ret = tabview.Draw(rect,index, tabnames, draw);
            GUI.DrawBorder(rectab, 1, GUIStyle.Current.BackgroundColorS1,true);

            AutoCaculateOffset(rect.Z, rect.W);
            return ret;
        }
        public static int TabViewVertical(int index, List<string> tabnames, Action<int> draw,int tabWidth, params GUIOption[] options)
        {
            var sizeRemain = GUILayout.SizeRemain;
            var rect = new Vector4(CurrentLayout.Offset, SizeRemain.X, SizeRemain.Y);
            if (options != null)
            {
                foreach (var opt in options)
                {
                    if (opt.type == GUIOption.GUIOptionType.width)
                    {
                        rect.Z = opt.IntValue;
                        continue;
                    }
                    if (opt.type == GUIOption.GUIOptionType.height)
                    {
                        rect.W = opt.IntValue;
                    }
                }
            }
            var rectab = GetRectAbsolute(rect);

            var tabview = GUI.GetTabView(rectab,(tv)=> {
                tv.SetVerticalMode(tabWidth);
            });

            int ret = tabview.Draw(rect, index, tabnames, draw);
            GUI.DrawBorder(rectab, 1, GUIStyle.Current.BackgroundColorS1, true);

            AutoCaculateOffset(rect.Z, rect.W);
            return ret;
        }


        public static T EnumPopup<T>(T selected) where T : struct, IConvertible
        {
            return selected;
        }

        public static int IntPopup(int selected,string[] values)
        {
            return selected;
        }

        public static void RadioGroup()
        {

        }

        public static void Progressbar()
        {

        }

        public static void SortableList()
        {

        }

        public static void Spinner()
        {

        }

        public static void ListView()
        {

        }

        public static void Tags()
        {

        }

        public static void ToolTip()
        {

        }

        #endregion

        #region Utility
        public static Vector4 GetRectAbsolute(Vector4 rect)
        {
            var off = s_ctx.currentArea.Rect.Pos();
            rect.X += off.X;
            rect.Y += off.Y;
            return rect;
        }
        #endregion
    }
}
