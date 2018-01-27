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
        public string ContextName;
        public Type ContextType;

        public RigelGfxContextAttribute(string contextName,Type contextType,GraphicsAPIEnum supportGraphics,PlatformEnum supportPlatform)
        {
            this.ContextName = contextName;
            this.ContextType = contextType;
            this.SupportGraphics = supportGraphics;
            this.SupportPlatform = supportPlatform;
        }
    }
}
