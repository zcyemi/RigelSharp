using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

using Rigel;
using Rigel.Engine;

namespace Rigel.Rendering
{
    public static class Graphics
    {
        private static bool m_inited = false;


        public static void Init(GraphicsContext context)
        {
            m_inited = true;
        }


        public static void DrawImmediate(Mesh mesh, Material material, Vector4 position, Quaternion rotation, Vector3 scale)
        {
            if (!m_inited) return;

        }
    }
}
