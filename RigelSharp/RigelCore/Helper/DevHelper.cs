using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore
{
    class DevHelper
    {
    }

    [AttributeUsage(AttributeTargets.All,AllowMultiple =true,Inherited = false)]
    public class TODOAttribute: Attribute
    {
        private string Tag;
        private string Msg;
        public TODOAttribute(string tag,string msg)
        {
            this.Tag = tag;
            this.Msg = msg;
        }
    }
}
