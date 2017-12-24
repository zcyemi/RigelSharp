using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Context
{
    public class RigelGfxContextAttribute :Attribute
    {
        public GraphicsAPIEnum SupportGraphics;
        public PlatformEnum SupportPlatform;

        public RigelGfxContextAttribute(GraphicsAPIEnum supportGraphics,PlatformEnum supportPlatform)
        {
            this.SupportGraphics = supportGraphics;
            this.SupportPlatform = supportPlatform;
        }
    }
}
