using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RigelEditor
{
    public static class RigelUtility
    {
        public static void Dispose(ref IDisposable disposable)
        {
            if (disposable != null) disposable.Dispose();
        }

        public static int SizeOf<T>()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf<T>();
        }

        public static void Log(string info)
        {
            Console.WriteLine(info);
        }
    }

    public static class RigelReflectionHelper
    {
        public static Assembly AssemblyRigelEditor;

        static RigelReflectionHelper()
        {
            AssemblyRigelEditor = Assembly.GetAssembly(typeof(RigelEditorApp));
        }

        public static List<MethodInfo> GetMethodByAttribute<T>(Type t,BindingFlags binding) where T : Attribute
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (var m in t.GetMethods(binding))
            {
                if (Attribute.IsDefined(m, typeof(T)))
                {
                    methods.Add(m);
                }
            }
            
            return methods;
        }
    }
}
