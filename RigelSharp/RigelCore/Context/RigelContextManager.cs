using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Context
{
    public static class RigelContextManager
    {
        private static Dictionary<string, RigelWindowContextAttribute> s_windowContextTypes = new Dictionary<string, RigelWindowContextAttribute>();
        private static Dictionary<string, RigelGfxContextAttribute> s_gfxContextTypes = new Dictionary<string, RigelGfxContextAttribute>();

        public static void CheckContextProvider()
        {
            ReflectionHelper.ForEachAssembly((asm) =>
            {
                var types = asm.GetTypes();
                foreach(var t in types)
                {
                    //WindowContext
                    {
                        var attr = ReflectionHelper.GetAttribute<RigelWindowContextAttribute>(t);
                        if (attr != null)
                            s_windowContextTypes.Add(attr.ContextName, attr);
                    }


                    //GfxContext
                    {
                        var attr = ReflectionHelper.GetAttribute<RigelGfxContextAttribute>(t);
                        if (attr != null)
                            s_gfxContextTypes.Add(attr.ContextName, attr);
                    }
                }

            });


            Console.WriteLine($"WindowCtx:{s_windowContextTypes.Count} GfxCtx:{s_gfxContextTypes.Count}");
        }


        public static IRigelWindowContext GetWindowContext(PlatformEnum platform,string contextName = null)
        {
            bool specificContext = !string.IsNullOrEmpty(contextName);
            if (specificContext)
            {
                if (s_windowContextTypes.ContainsKey(contextName))
                {
                    var attr = s_windowContextTypes[contextName];
                    if (platform == attr.SupportPlatform)
                    {
                        return (IRigelWindowContext)Activator.CreateInstance(attr.ContextType);
                    }
                }
            }

            foreach(var attr in s_windowContextTypes.Values)
            {
                if(attr.SupportPlatform == platform)
                {
                    return (IRigelWindowContext)Activator.CreateInstance(attr.ContextType);
                }
            }

            return null;
        }

        public static IRigelGfxContext GetGfxContext(GraphicsAPIEnum curGraphics,PlatformEnum platform,string contextName = null)
        {
            bool specificContext = !string.IsNullOrEmpty(contextName);
            if (specificContext)
            {
                if (s_gfxContextTypes.ContainsKey(contextName))
                {
                    var attr = s_gfxContextTypes[contextName];
                    if (platform == attr.SupportPlatform && curGraphics == attr.SupportGraphics)
                    {
                        return (IRigelGfxContext)Activator.CreateInstance(attr.ContextType);
                    }
                }
            }

            foreach (var attr in s_gfxContextTypes.Values)
            {
                if (platform == attr.SupportPlatform && curGraphics == attr.SupportGraphics)
                {
                    return (IRigelGfxContext)Activator.CreateInstance(attr.ContextType);
                }
            }

            return null;
        }
    }
}
