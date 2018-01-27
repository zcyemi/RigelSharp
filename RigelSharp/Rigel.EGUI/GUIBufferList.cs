using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.EGUI
{
    public abstract class GUIBufferList<T> where T : struct
    {
        protected List<T> m_data = new List<T>();
    }
}
