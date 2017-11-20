using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;
using RigelCore.Engine;


namespace RigelCore.Rendering
{
    public enum CommandBufferStage
    {
        Default,
        PreRender,
        PostRender,
    }

    public class CommandBuffer
    {

        public void ClearRenderTarget(Vector4 color)
        {

        }

        public void Draw(Mesh mesh,Material material)
        {

        }
    }
}
