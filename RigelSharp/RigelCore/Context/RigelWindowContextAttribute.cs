using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Context
{
    public class RigelWindowContextAttribute : Attribute
    {
        public string ContextName;
        public Type ContextType;
        public PlatformEnum SupportPlatform;

        public RigelWindowContextAttribute(string contextname,Type contexttype,PlatformEnum supportPlatform)
        {
            this.ContextName = contextname;
            this.ContextType = contexttype;
            this.SupportPlatform = supportPlatform;
        }
    }
}
