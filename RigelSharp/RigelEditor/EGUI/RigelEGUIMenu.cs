using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RigelEditor.EGUI
{
    public class RigelEGUIMenuItemNode
    {
        public string Label;
        public object Func;
        public RigelEGUIMenuItemNode(string label,Action func)
        {
            Label = label;
            Func = func;
        }

        public RigelEGUIMenuItemNode(string label, MethodInfo method)
        {
            Label = label;
            Func = method;
        }

        public void Invoke()
        {
            if (Func == null) return;
            if(Func is Action)
            {
                (Func as Action).Invoke();
            }
            else
            {
                (Func as MethodInfo).Invoke(null, null);
            }
        }

    }


    public class RigelEGUIMenu
    {
        private List<RigelEGUIMenuItemNode> m_menuItemNode;
        public List<RigelEGUIMenuItemNode> ItemNodes { get { return m_menuItemNode; } }

        public RigelEGUIMenu()
        {
            m_menuItemNode = new List<RigelEGUIMenuItemNode>();
        }

        public void AddMenuItem(string label, Action func)
        {
            foreach(var node in m_menuItemNode)
            {
                if (node.Label == label) return;
            }
            m_menuItemNode.Add(new RigelEGUIMenuItemNode(label, func));
        }

        public void AddMenuItem(string label, MethodInfo method)
        {
            foreach (var node in m_menuItemNode)
            {
                if (node.Label == label) return;
            }
            m_menuItemNode.Add(new RigelEGUIMenuItemNode(label, method));
        }

        public void ClearAllItems()
        {
            m_menuItemNode.Clear();
        }
    }

    public class RigelEGUIMenuItemAttribute : Attribute
    {
        public string Label;
        public RigelEGUIMenuItemAttribute(string label)
        {
            Label = label;
        }
    }
}
