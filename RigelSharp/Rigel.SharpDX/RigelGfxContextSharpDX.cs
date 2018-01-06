using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Context;
using Rigel.Rendering;

namespace Rigel.SharpDX
{
    [RigelGfxContext(RigelSharpDX.CONTEXT_NAME, typeof(RigelGfxContextSharpDX), GraphicsAPIEnum.DirectX, PlatformEnum.Windows)]
    public class RigelGfxContextSharpDX : IRigelGfxContext
    {
        public GraphicsContextBase CreateGraphicsContext(object initobj)
        {
            GraphicsContextSharpDX gctx = new GraphicsContextSharpDX();
            return gctx;
        }
    }
}
