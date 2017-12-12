using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

    public interface IFontInfo
    {
        int GetTextWidth(string content);
        float UVUnit { get; }
        GlyphInfo GetGlyphInfo(uint c);
        uint FontPixelSize { get; }
        int GetCharWidth(uint c);

        object GetMaterial();
    }
}
