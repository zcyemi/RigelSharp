using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{


    public static partial class GUI
    {
        public static GUICtx s_ctx;
        public static GUICtx Context
        {
            get { return s_ctx; }
            set { s_ctx = value; GUILayout.s_ctx = s_ctx; }
        }
        public static GUIDrawTarget DrawTarget { get; private set; }

        public static GUIEvent Event { get; set; }

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
            EditorUtility.Assert(groupStack.Count > 0);
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
        }

        public static bool Button(Vector4 rect, string label, bool absolute = false, params GUIOption[] options)
        {
            return Button(rect, label, Context.BackgroundColor, Context.Color, absolute, options);
        }
        public static bool Button(Vector4 rect, string label, Vector4 color, Vector4 texcolor, bool absolute = false, params GUIOption[] options)
        {
            GUIOption optAdaptiveValue = null;
            if (options != null)
            {
                optAdaptiveValue = options.FirstOrDefault((o) => { return o.type == GUIOption.GUIOptionType.adaptiveValue; });
                if (optAdaptiveValue != null)
                {
                    int textWidth = Context.Font.GetTextWidth(label);
                    rect.Z = Math.Min(textWidth + 6,optAdaptiveValue.IntValue);
                    optAdaptiveValue.value = (int)rect.Z;
                }
            }

            var groupAb = s_ctx.currentGroup.Absolute;

            if (!absolute)
            {
                float maxwidth = groupAb.Z - rect.X;
                if (rect.Z > maxwidth) rect.Z = maxwidth;
                rect.X += groupAb.X;
                rect.Y += groupAb.Y;
                absolute = true;
            }

            

            float depthz = GUI.Depth;
            GUI.DepthIncrease();

            int textw = DrawText(rect, label, texcolor, absolute, options);
            depthz = GUI.SetDepth(depthz);

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

                if (Event.Used && !clicked)
                {
                    DrawRect(rect, color, absolute);
                }
                else if (Event.Button == MouseButton.Left)
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


            GUI.SetDepth(depthz);

            return clicked;
        }

        public static void Label(Vector4 position, string text, bool absolute = false)
        {
            DrawText(position, text, Context.Color, absolute);
        }

        /// <summary>
        /// Multiline Text
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="absoulte"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int TextBlock(Vector4 rect, string text, Vector4 color, bool absoulte = false, params GUIOption[] options)
        {
            bool valid = GUIUtility.RectClip(ref rect, absoulte ? s_ctx.baseRect : s_ctx.currentGroup.Absolute);

            if (!valid)
            {
                return (int)rect.W;
            }
            Vector4 startpos = rect;


            uint lineh = Context.Font.FontPixelSize + 3;
            int w = 0;
            foreach (var c in text)
            {
                int cw = Context.Font.GetCharWidth(c);
                if ((w + cw) > rect.Z)
                {
                    startpos.Y += lineh;
                    w = 0;

                    if ((startpos.Y + lineh) > (rect.W + rect.Y)) return (int)rect.W;
                }
                startpos.X = w + rect.X;
                w += DrawChar(startpos, c, color);

            }

            return (int)(startpos.Y + lineh - rect.Y);
        }

        public static int DrawText(Vector4 rect, string content, Vector4 color, bool absolute = false, params GUIOption[] options)
        {
            bool adaptive = false;
            int alignH = 0;     //-1 Left   0 None  1 Right
            int alignV = 0;     //-1 Top    0 None  1 Bottom

            GUIOption optTextClip = null;
            if (options != null)
            {
                foreach (var o in options)
                {
                    if (o.type == GUIOption.GUIOptionType.adaptive) { adaptive = true; continue; }
                    if (o == GUIOption.AlignHLeft) { alignH = -1; continue; }
                    if (o == GUIOption.AlignHRight) { alignH = 1; continue; }
                    if (o == GUIOption.AlignVTop) { alignV = -1; continue; }
                    if (o == GUIOption.AlignVBottom) { alignV = 1; continue; }
                    if(o.type == GUIOption.GUIOptionType.textClip) { optTextClip = o; continue; }
                }
            }

            if(alignV == 0)
            {
                int pixelsize = (int)Context.Font.FontPixelSize;
                rect.Y += rect.W > pixelsize ? (rect.W - pixelsize) / 2 : 0;
            }
            else if(alignV == 1)
            {
                int pixelsize = (int)Context.Font.FontPixelSize;
                rect.Y += ((rect.W - pixelsize) - 1);
            }
            else
            {
                rect.Y += 1;
            }
            

            bool valid = GUIUtility.RectClip(ref rect, absolute ? s_ctx.baseRect : s_ctx.currentGroup.Absolute);
            if (!valid) return 0;
            int w = 0;

            //centered
            if (adaptive)
            {
                rect.X += 3;    //default offset;
                w += 3;
                rect.Z -= 3;
            }
            else
            {
                int textwidth = Context.Font.GetTextWidth(content);
                if (alignH == 0)
                {
                    w = rect.Z > textwidth ? (int)((rect.Z - textwidth) / 2) : 3;
                }
                else if (alignH == -1)
                {
                    w = 3;
                }
                else
                {
                    w = (int)rect.Z - textwidth - 3;
                }
                rect.X += w;
                rect.Z -= w;
            }

            if (adaptive)
            {
                foreach (var c in content)
                {
                    int xoff = DrawChar(rect, c, color);
                    w += xoff;
                    rect.X += xoff;
                    rect.Z -= xoff;
                    if (rect.Z < 3) break;
                }
                w += 3;
            }
            else
            {
                int clipoff = 0;
                if(optTextClip != null)
                {
                    clipoff = (int)optTextClip.Vector4Value.X;
                }
                foreach (var c in content)
                {
                    if(clipoff < 0)
                    {
                        if (c < 33)
                        {
                            clipoff += 6;
                            if(clipoff > 0)
                            {
                                w += 6;
                            }
                            continue;
                        }
                        int cw = Context.Font.GetGlyphInfo(c).AdvancedX;
                        clipoff += cw;
                        if(clipoff < 0)
                        {
                            continue;
                        }
                        else
                        {
                            w += clipoff;
                        }
                    }

                    int xoff = DrawChar(rect, c, color);
                    w += xoff;
                    rect.X += xoff;
                    rect.Z -= xoff;
                    if (rect.Z < 3) break;
                }
            }
            return w;
        }
        public static int DrawChar(Vector4 rect, uint c, Vector4 color)
        {
            if (c < 33)
            {
                return 6;
            }
            var glyph = Context.Font.GetGlyphInfo(c);
            if (glyph == null) return (int)Context.Font.FontPixelSize;

            float x1 = rect.X + glyph.LineOffsetX;
            float y1 = rect.Y + glyph.LineOffsetY;
            float x2 = Math.Min(rect.X+ rect.Z,x1 + glyph.PixelWidth);
            float y2 = Math.Min(rect.Y+ rect.W,y1 + glyph.PixelHeight);

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

        #region Text
        //  |               | Rect Absolute  |  Rect Relative    |
        //  | Current Group |       -        |      GUI.Text     |
        //  | Root Group    |   GUI.TextA    |          -        |

        /// <summary>
        /// Draw text inside rect,rect is clipped by currentGroup
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="content"></param>
        /// <param name="color"></param>
        public static void Text(Vector4 rect,string content,Vector4? color= null)
        {
            //Vector2 pos = Vector2.Zero;
            //pos.Y = (int)((rect.W - GUI.Context.Font.FontPixelSize) * 0.5f);
            //if (pos.Y < 0) pos.Y = 0;

            //var rectdraw = rect;
            //GUIUtility.RectClip(ref rectdraw, GUI.Context.currentGroup.Rect);


            

            Vector2 startpos = rect.Pos() + GUI.Context.currentGroup.Absolute.Pos();
            bool valid = GUIUtility.RectClip(ref rect, GUI.Context.currentGroup.Absolute);
            if (valid)
                _ImplDrawTextA(rect, startpos - rect.Pos(), content, color?? Context.Color);
        }

        public static void TextA(Vector4 recta,string content,Vector4? color = null)
        {
            GUI._ImplDrawTextA(recta, Vector2.Zero, content, color ?? Context.Color);
        }

        public static void TextA(Vector4 recta,Vector2 pos, string content, Vector4? color = null)
        {
            GUI._ImplDrawTextA(recta, pos, content, color ?? Context.Color);
        }


#endregion

        public static void DrawRect(Vector4 rect, bool absolute = false, params GUIOption[] options)
        {
            DrawRect(rect, Context.BackgroundColor, absolute, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="absolute"></param>
        /// <param name="options">border noClip</param>
        public static void DrawRect(Vector4 rect, Vector4 color, bool absolute = false, params GUIOption[] options)
        {
            GUIOption optBorder = null;
            if (options != null)
            {
                var noclip = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.noClip; });
                if (noclip == null)
                {
                    var valid = GUIUtility.RectClip(ref rect, absolute ? s_ctx.baseRect : s_ctx.currentGroup.Absolute);
                    if (!valid) return;
                }
                else
                {
                    rect = rect.Move(absolute ? s_ctx.baseRect.Pos() : s_ctx.currentGroup.Absolute.Pos());
                }

                optBorder = options.FirstOrDefault((x) => { return x.type == GUIOption.GUIOptionType.border; });
            }

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

            if (optBorder != null)
            {
                DrawBorder(rect, 1, (Vector4)optBorder.value, true);
            }

            DepthIncrease();
        }
        public static void DrawTexture(Vector4 rect, uint textureid)
        {

        }

        public static void DrawBorder(Vector4 rect, int thickness, Vector4? color, bool absolute = false)
        {
            var p0 = rect.Pos();
            var p2 = p0 + rect.Size();
            var p1 = p0; p1.Y = p2.Y;
            var p3 = p0; p3.X = p2.X;

            DrawLineAxisAligned(p0, p1, thickness, color, absolute);
            DrawLineAxisAligned(p1, p2, thickness, color, absolute);
            DrawLineAxisAligned(p3, p2, thickness, color, absolute);
            DrawLineAxisAligned(p0, p3, thickness, color, absolute);
        }

        public static void DrawLine(Vector2 startp, Vector2 endp, int thickness, bool absolute = false)
        {

        }

        public static void DrawLineAxisAligned(Vector2 startp, Vector2 endp, int thickness, Vector4? color, bool absolute = false)
        {
            var rect = new Vector4(startp, endp.X - startp.X, endp.Y - startp.Y);

            float thickhalf = thickness / 2.0f;
            if (rect.Z > rect.W)
            {
                rect.W = thickness;
                rect.Z += thickhalf;
            }
            else
            {
                rect.Z = thickness;
                rect.W += thickhalf;
            }

            rect.X -= thickhalf;
            rect.Y -= thickhalf;
            if (color != null)
            {
                DrawRect(rect, (Vector4)color, absolute);
            }
            else
            {
                DrawRect(rect, absolute);
            }
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
            component.Destroy = false;
            Context.componentStack.Push(component);
            Event.Use();
        }

        #region Input
        public static bool Toggle(Vector4 rect, bool value, string label)
        {
            DrawRect(rect);
            return value;
        }

        #endregion





        #region ObjectPool

        public static GUIObjScrollView GetScrollView(Vector4 rect)
        {
            return Context.poolSrollbar.Get(GUIUtilityInternal.GetHash(rect, GUIObjectType.ScrollBar));
        }

        public static GUIObjTextInput GetTextInput(Vector4 rect)
        {
            return Context.poolTextInput.Get(GUIUtilityInternal.GetHash(rect, GUIObjectType.TextInput));
        }

        public static GUIObjectTabView GetTabView(Vector4 rect,Action<GUIObjectTabView> createFunction = null)
        {
            return Context.poolTabView.Get(GUIUtilityInternal.GetHash(rect, GUIObjectType.TabView),createFunction);
        }

        #endregion


        #region utility

        /// <summary>
        /// don't use nested Begin/End ChangeCheck
        /// </summary>
        public static void BeginChangeCheck()
        {
            s_ctx.InputChanged = false;
        }

        public static bool EndChangeCheck()
        {
            var ret = s_ctx.InputChanged;
            s_ctx.InputChanged = false;

            return ret;
        }


        internal static void SetDrawTarget(GUIDrawTarget target)
        {
            DrawTarget = target;
            s_depthz = target.depth;
        }
        internal static float DepthIncrease()
        {
            float z = s_depthz;
            s_depthz -= Context.s_depthStep;
            return z;
        }

        internal static float SetDepth(float depth)
        {
            float retd = s_depthz;
            s_depthz = depth;
            return retd;
        }


        public static Vector4 GetRectAbsolute(Vector4 rect)
        {
            var off = Context.currentGroup.Absolute.Pos();
            rect.X += off.X;
            rect.Y += off.Y;

            return rect;
        }

        /// <summary>
        /// depth layer from 1 - 9
        /// </summary>
        /// <param name="layer"></param>
        public static void BeginDepthLayer(int layer)
        {
            if (layer < 1 || layer > 9)
            {
                throw new Exception("GUI DepthLayer invalid (1-9)");
            }

            int layeroffset = layer - (s_ctx.depthLayer.Count == 0 ? 0 : s_ctx.depthLayer.Peek());
            s_depthz -= layeroffset * 0.1f;
            s_ctx.depthLayer.Push(layer);
        }
        public static void EndDepthLayer()
        {
            int layeroffset = s_ctx.depthLayer.Pop() - (s_ctx.depthLayer.Count == 0 ? 0 : s_ctx.depthLayer.Peek());
            s_depthz += layeroffset * 0.1f;
        }


        #endregion
    }
}
