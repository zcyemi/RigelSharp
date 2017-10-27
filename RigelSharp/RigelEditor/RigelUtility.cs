using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Diagnostics;

using SharpDX;

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
            //Console.WriteLine(info);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void Assert(bool condition, string message = null, RigelAssertLevel level = RigelAssertLevel.Fatel)
        {
            if(level >= RigelConfig.AssertLevel)
                Debug.Assert(condition, message);
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

    public static class VectorExtension
    {
        public static Vector2 Pos(this Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 Size(this Vector4 v)
        {
            return new Vector2(v.Z, v.W);
        }
    }

}
