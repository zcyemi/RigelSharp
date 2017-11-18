using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RigelEditor
{
    public class EditorReflectionHelper
    {
        public static Assembly AssemblyRigelEditor;

        static EditorReflectionHelper()
        {
            AssemblyRigelEditor = Assembly.GetAssembly(typeof(RigelEditorApp));
        }

    }
}
