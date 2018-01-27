using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Engine
{
    public abstract class BaseComponent
    {
        internal GObject m_gobj = null;
        public GObject gobj { get { return m_gobj; } }


        public virtual void Awake()
        {

        }

        public virtual void OnUpdate()
        {

        }
        public virtual void OnDestroy()
        {

        }
        
    }
}
