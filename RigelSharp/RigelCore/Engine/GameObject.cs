using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Engine
{
    public class GameObject : GObject
    {
        private Transform m_transform = new Transform();
        public Transform transform { get { return m_transform; } }


    }
}
