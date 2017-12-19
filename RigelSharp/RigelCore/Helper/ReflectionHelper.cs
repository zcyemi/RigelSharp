
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
    }
}
