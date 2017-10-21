using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpFont;

namespace RigelEditor
{
    public class RigelFont : IDisposable
    {
        private static Library s_ftLibrary;

        static RigelFont()
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

        public string FontFilePath { get; private set; }
        private Face m_ftface = null;
        public uint FontPixelSize { get; private set; }

        public RigelFont(string fontpath, uint pixelsize = 12)
        {
            FontFilePath = fontpath;
            FontPixelSize = pixelsize;
            m_ftface = new Face(s_ftLibrary, fontpath);
            m_ftface.SetPixelSizes(FontPixelSize, FontPixelSize);
        }



        public void GenerateFontTexture(RigelImageData img)
        {
            int lineposx = 0;
            int lineposy = 0;
            int linehmax = 0;

            for(uint c = 33; c < 127; c++)
            {
                DrawGlyphToImage(c, img, lineposx, lineposy);

                var bitmap = m_ftface.Glyph.Bitmap;
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

        public void DrawGlyphToImage(uint c,RigelImageData img,int posx,int posy)
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
