
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Rigel
{
    public static class ReflectionHelper
    {
        public static List<MethodInfo> GetMethodByAttribute<T>(Type t, BindingFlags binding) where T : Attribute
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

        public static void ForEachAssembly(Action<Assembly> method)
        {
            if (method == null) return;
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            for(int i=0;i< asms.Length; i++)
            {
                method(asms[i]);
            }
        }

        public static T GetAttribute<T>(Type t) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(t, typeof(T));
        }

        public static List<T> GetAttributes<T>(Type t) where T : Attribute
        {
            List<T> ret = new List<T>();
            var attrs = Attribute.GetCustomAttributes(t);
            if (attrs == null) return ret;
            foreach(var attr in attrs)
            {
                if(attr is T)
                {
                    ret.Add((T)attr);
                }
            }
            return ret;
        }
    }
}
