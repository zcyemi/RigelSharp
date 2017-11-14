using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor
{

    public class EventSlot<T>
    {
        private List<T> m_involist = new List<T>();

        public static EventSlot<T> operator +(EventSlot<T> slot, T c)
        {
            slot.m_involist.Add(c);
            return slot;
        }

        public static EventSlot<T> operator -(EventSlot<T> slot,T c)
        {
            if (slot.m_involist.Contains(c))
            {
                slot.m_involist.Remove(c);
            }
            return slot;
        }

        public void Invoke()
        {
            foreach(var o in m_involist)
            {
                (o as Delegate).DynamicInvoke(null);
            }
        }

        public void InvokeOnce()
        {
            Invoke();
            m_involist.Clear();
        }
    }
}
