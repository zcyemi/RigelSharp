using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor
{
    public enum RigelAssertLevel :int
    {
        Exception = 1,
        Fatel = 2,
    }

    public static class EditorConfig
    {
        public static RigelAssertLevel AssertLevel = RigelAssertLevel.Exception;
    }
}
