using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelCore.Engine
{
    public class Mesh
    {
        public Vector4[] Vertices;
        public int[] Indices;

        public Vector4[] Normal;
        public Vector4[] Color;

        public Vector4[] UV0;
        public Vector4[] UV1;
        public Vector4[] UV2;
        public Vector4[] UV3;
    }
}
