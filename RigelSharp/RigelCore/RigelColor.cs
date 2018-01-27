using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace Rigel
{
    public class RigelColor
    {
        private static System.Random s_random;
        static RigelColor()
        {
            s_random = new System.Random();
        }

        public static readonly Vector4 White = new Vector4(1, 1, 1, 1);
        public static readonly Vector4 Black = new Vector4(0, 0, 0, 1);
        public static readonly Vector4 Red = new Vector4(1, 0, 0, 1);
        public static readonly Vector4 Green = new Vector4(0, 1, 0, 1);
        public static readonly Vector4 Blue = new Vector4(0, 0, 1, 1);

        public static Vector4 Random()
        {
            return new Vector4((float)s_random.NextDouble(), (float)s_random.NextDouble(), (float)s_random.NextDouble(),(float)s_random.NextDouble());
        }

        public static Vector4 RGBA(byte r, byte g, byte b, byte a)
        {
            return new Vector4(r, g, b, a) / 255.0f;
        }
    }

}
