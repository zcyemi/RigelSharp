using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore.Engine
{
    public class GObject : Object
    {
        internal List<BaseComponent> m_component;

        public T AddComponent<T>() where T: BaseComponent
        {
            T comp = Activator.CreateInstance<T>();
            if(m_component == null)
            {
                m_component = new List<BaseComponent>();
            }

            comp.m_gobj = this;
            m_component.Add(comp);

            return comp;
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            foreach(var comp in m_component)
            {
                if (comp.GetType() == typeof(T)) return (T)comp;
            }
            return null;
        }

    }
}
