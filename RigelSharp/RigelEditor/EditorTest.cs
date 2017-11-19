using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor
{
    class EditorTest
    {
        [EditorMenuItem("Test","EditorTest")]
        public static void RunTestScript()
        {
            Console.WriteLine((int)' ');
        }
    }
}
