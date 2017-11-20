using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;

namespace RigelEditor
{
    public interface IEditorModule
    {
        void Init();
        void Update();

        void Dispose();
    }

    public class EditorModuleManager:Singleton<EditorModuleManager>
    {
        private List<IEditorModule> m_modules = new List<IEditorModule>();

        public void Init()
        {
            GetAllModules();
            Console.WriteLine("ModuleCount:"+m_modules.Count);

            foreach(var module in m_modules)
            {
                module.Init();
            }
        }

        private void GetAllModules()
        {
            var types = EditorReflectionHelper.AssemblyRigelEditor.GetTypes();
            Type moduleInterface = typeof(IEditorModule);
            foreach (var t in types)
            {
                if (t.GetInterface(moduleInterface.ToString()) != null)
                {
                    IEditorModule o = Activator.CreateInstance(t) as IEditorModule;
                    m_modules.Add(o);
                }
            }
        }

        public void Update()
        {
            foreach (var module in m_modules)
            {
                module.Update();
            }
        }

        public T FindModule<T>() where T: class,IEditorModule
        {
            foreach(var m in m_modules)
            {
                if(m is T)
                {
                    return (T)m;
                }
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var module in m_modules)
            {
                module.Dispose();
            }
        }
    }
}
