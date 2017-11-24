using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore.Engine
{
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = false,Inherited =false)]
    public class GameScriptAttribute :Attribute
    {
        public Type entryType;
        public GameScriptAttribute(Type t) 
        {
            entryType = t;
        }
    }
}
