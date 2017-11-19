using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpFont;
using SharpDX;

using RigelCore;

namespace RigelCore
{

    public class GlyphInfo
    {
        public int PixelWidth;
        public int PixelHeight;
        public int PosX;
        public int PosY;

        public int LineOffsetX;
        public int LineOffsetY;

        public int AdvancedX;

        public Vector2[] UV;

        public void UpdateUVData(int texsize)
        {
            UV = new Vector2[4];

            float uvunit = 1.0f / texsize;

            float w = PixelWidth * uvunit;
            float h = PixelHeight * uvunit;

            UV[0].X = PosX * uvunit;
            UV[0].Y = PosY * uvunit;

            UV[1].X = UV[0].X;
            UV[1].Y = UV[0].Y + h;

            UV[3].X = UV[0].X + w;
            UV[3].Y = UV[0].Y;

            UV[2].X = UV[3].X;
            UV[2].Y = UV[1].Y;
        }


    }

    public class FontInfo : IDisposable
    {
        private static Library s_ftLibrary;

        static FontInfo()
        {
            s_ftLibrary = new Library();
        }

        public static void ReleaseResource()
        {
            s_ftLibrary.Dispose();
        }

        public void Dispose()
        {
            if(m_ftface != null)
            {
                m_ftface.Dispose();
            }
            
        }

        public GlyphInfo GetGlyphInfo(uint c)
        {
            return m_glyphInfo[c];
        }

        public int GetCharWidth(uint c)
        {
            return m_charWidth[c];
        }

        public int GetTextWidth(string str)
        {
            int width = 0;
            foreach(char c in str)
            {
                if(c < 33)
                {
                    width += 6;
                    continue;
                }
                width += GetGlyphInfo(c).AdvancedX;
            }

            return width;
        }
        

        public string FontFilePath { get; private set; }
        private Face m_ftface = null;
        public uint FontPixelSize { get; private set; }
        private int m_textureSize;
        private float m_glyphUV;
        private GlyphInfo[] m_glyphInfo = new GlyphInfo[128];
        private int[] m_charWidth = new int[128];

        private int m_ascender;

        public FontInfo(string fontpath, uint pixelsize = 13)
        {
            FontFilePath = fontpath;
            FontPixelSize = pixelsize;
            m_ftface = new Face(s_ftLibrary, fontpath);
            m_ftface.SetPixelSizes(FontPixelSize, FontPixelSize);

            m_ascender = m_ftface.Size.Metrics.Ascender.Value >> 6;

        }



        public void GenerateFontTexture(ImageData img)
        {

            m_textureSize = img.Width;
            m_glyphUV = 1.0f / m_textureSize;

            int lineposx = 0;
            int lineposy = 0;
            int linehmax = 0;

            for(uint c = 0;c< 33; c++)
            {
                m_charWidth[c] = 6;
            }

            for (uint c = 33; c < 127; c++)
            {
                DrawGlyphToImage(c, img, lineposx, lineposy);


                var bitmap = m_ftface.Glyph.Bitmap;

                m_glyphInfo[c] = new GlyphInfo()
                {
                    AdvancedX = m_ftface.Glyph.Advance.X.Value >> 6,
                    LineOffsetY = m_ascender - m_ftface.Glyph.BitmapTop,
                    LineOffsetX = m_ftface.Glyph.BitmapLeft,
                    PosX = lineposx,
                    PosY = lineposy,
                    PixelWidth = bitmap.Width,
                    PixelHeight = bitmap.Rows,
                };

                m_charWidth[c] = m_glyphInfo[c].AdvancedX;
                m_glyphInfo[c].UpdateUVData(m_textureSize);

                linehmax = bitmap.Rows > linehmax ? bitmap.Rows : linehmax;

                lineposx += bitmap.Width;
                if(lineposx > img.Width - FontPixelSize)
                {
                    lineposx = 0;
                    lineposy += linehmax;
                    linehmax = 0;
                }
            }
        }

        public void DrawGlyphToImage(uint c,ImageData img,int posx,int posy)
        {
            m_ftface.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
            var bitmap = m_ftface.Glyph.Bitmap;
            int imgdepth = (int)img.Depth;
            for (int y = 0; y < bitmap.Rows; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int index = imgdepth * ((y + posy) * img.Width + x + posx);
                    byte v = bitmap.BufferData[x + y * bitmap.Width];
                    img.Data[index] = v;
                    img.Data[index+1] = v;
                    img.Data[index+2] = v;
                    if(imgdepth == 4) img.Data[index + 3] = v;
                }
            }
        }
    }
}
