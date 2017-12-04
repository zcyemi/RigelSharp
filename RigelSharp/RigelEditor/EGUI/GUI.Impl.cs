using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;
using SharpDX;

namespace RigelEditor.EGUI
{
    public static partial class GUI
    {
        
        /// <summary>
        /// Draw text inside [reacta] with offset value [pos].
        /// </summary>
        /// <param name="recta"></param>
        /// <param name="pos"></param>
        /// <param name="content"></param>
        /// <param name="color"></param>
        public static void _ImplDrawTextA(Vector4 recta, Vector2 pos, string content, Vector4 color)
        {
            //debug draw
            //DrawRect(recta, RigelCore.RigelColor.Red, true);

            int count = 0;
            Vector2 startpos = pos;

            if (pos.Y >= recta.W) return;
            if ((pos.Y + Context.Font.FontPixelSize) <= 0) return;

            bool yclip = true;
            if (pos.Y > recta.Y && (pos.Y + Context.Font.FontPixelSize) < recta.W)
            {
                yclip = false;
            }

            bool bnoclip = false;

            foreach (var c in content)
            {
                if (c < 33)
                {
                    startpos.X += 6;
                    continue;
                }
                var cw = Context.Font.GetCharWidth(c);
                if (startpos.X + cw < 0)
                {
                    startpos.X += cw;
                    continue;
                }
                if (!bnoclip)
                {
                    if (startpos.X >= 0) bnoclip = true;
                    startpos.X += _ImplDrawCharWithRect(recta, startpos, c, color);
                    count++;
                }
                else
                {
                    if (startpos.X >= recta.Z)
                    {
                        return;
                    }
                    if (!yclip && startpos.X + cw <= recta.Z)
                    {
                        startpos.X += _ImplDrawCharWithRect(recta, startpos, c, color, true);
                        count++;
                    }
                    else
                    {
                        startpos.X += _ImplDrawCharWithRect(recta, startpos, c, color);
                        count++;
                    }
                }
            }
        }

        public static int _ImplDrawCharWithRect(Vector4 recta, Vector2 pos, uint c, Vector4 color, bool noclip = false)
        {
            return _ImplDrawCharWithRectA(recta, pos + recta.Pos(), c, color, noclip);
        }
        public static int _ImplDrawCharWithRectA(Vector4 recta, Vector2 posa, uint c, Vector4 color, bool noclip = false)
        {
            if (c < 33) return 6;

            var glyph = Context.Font.GetGlyphInfo(c);

            Vector4 charrect = new Vector4(posa, glyph.PixelWidth, glyph.PixelHeight);
            charrect.X += glyph.LineOffsetX;
            charrect.Y += glyph.LineOffsetY;

            float x2 = charrect.X + charrect.Z;
            float y2 = charrect.Y + charrect.W;

            float rectx2 = recta.X + recta.Z;
            float recty2 = recta.Y + recta.W;

            if (x2 <= recta.X || charrect.X >= rectx2 || y2 <= recta.Y || charrect.Y >= recty2)
            {
                return glyph.AdvancedX;
            }


            if (!noclip)
            {
                Vector4 clipsize = Vector4.Zero;
                if (charrect.X < recta.X)
                {
                    clipsize.X = recta.X - charrect.X;
                    charrect.X = recta.X;
                }
                if (x2 > rectx2)
                {
                    clipsize.Z = x2 - rectx2;
                    x2 = rectx2;
                }

                if (charrect.Y < recta.Y)
                {
                    clipsize.Y = recta.Y - charrect.Y;
                    charrect.Y = recta.Y;
                }

                if (y2 > recty2)
                {
                    clipsize.W = y2 - recty2;
                    y2 = recty2;
                }
                clipsize *= Context.Font.UVUnit;

                Vector2 uv0 = glyph.UV[0];
                Vector2 uv1 = glyph.UV[1];
                Vector2 uv2 = glyph.UV[2];
                Vector2 uv3 = glyph.UV[3];

                uv0.X += clipsize.X;
                uv1.X += clipsize.X;
                uv2.X -= clipsize.Z;
                uv3.X -= clipsize.Z;

                uv0.Y += clipsize.Y;
                uv3.Y += clipsize.Y;
                uv1.Y -= clipsize.W;
                uv2.Y -= clipsize.W;

                //LT
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(charrect.X, charrect.Y, s_depthz, 1),
                    Color = color,
                    UV = uv0
                });
                //LB
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(charrect.X, y2, s_depthz, 1),
                    Color = color,
                    UV = uv1
                });
                //RB
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(x2, y2, s_depthz, 1),
                    Color = color,
                    UV = uv2
                });
                //RT
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(x2, charrect.Y, s_depthz, 1),
                    Color = color,
                    UV = uv3
                });

            }
            else
            {
                //LT
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(charrect.X, charrect.Y, s_depthz, 1),
                    Color = color,
                    UV = glyph.UV[0]
                });
                //LB
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(charrect.X, y2, s_depthz, 1),
                    Color = color,
                    UV = glyph.UV[1]
                });
                //RB
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(x2, y2, s_depthz, 1),
                    Color = color,
                    UV = glyph.UV[2]
                });
                //RT
                BufferText.Add(new RigelEGUIVertex()
                {
                    Position = new Vector4(x2, charrect.Y, s_depthz, 1),
                    Color = color,
                    UV = glyph.UV[3]
                });
            }

            return glyph.AdvancedX;
        }

        public static void _ImplDrawRectA(Vector4 recta,Vector4 color)
        {
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(recta.X, recta.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(recta.X, recta.Y + recta.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(recta.X + recta.Z, recta.Y + recta.W, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });
            BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(recta.X + recta.Z, recta.Y, s_depthz, 1),
                Color = color,
                UV = Vector2.Zero
            });

            DepthIncrease();
        }
    }
}
