using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using RigelCore;

using RigelEditor.EGUI;

namespace RigelEditor
{

    public class EditorMenuManager :Singleton<EditorMenuManager>
    {

        public List<GUIMenuList> m_menulist = new List<GUIMenuList>();

        private static readonly Dictionary<string, int> DefaultOrder;


        static EditorMenuManager()
        {
            DefaultOrder = new Dictionary<string, int>() {
                {"File",5000},
                {"About",1000}
            };
        }

        public EditorMenuManager()
        {
            UpdateEditorMenuItems();
        }

        public void OnDrawMainMenuBar()
        {
            if (m_menulist.Count == 0) return;
            foreach(var menulist in m_menulist)
            {
                GUILayout.DrawMenuList(menulist, GUIOption.Width(70));
            }
        }

        private void UpdateEditorMenuItems()
        {
            var types = EditorReflectionHelper.AssemblyRigelEditor.GetTypes();

            Type typeMenuItem = typeof(EditorMenuItemAttribute);
            foreach(var t in types)
            {
                var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach(var m in methods)
                {
                    if (m.GetParameters().Length != 0) continue;

                    if (Attribute.IsDefined(m, typeMenuItem))
                    {
                        AddMenuItem(Attribute.GetCustomAttribute(m, typeMenuItem) as EditorMenuItemAttribute, m);
                    }
                }
            }

            m_menulist.Sort((a, b) => { return b.Order.CompareTo(a.Order); });
        }

        private void SetListOrder(GUIMenuList list)
        {
            if (DefaultOrder.ContainsKey(list.Label))
            {
                list.Order = DefaultOrder[list.Label];
            }
            else
            {
                list.Order = 0;
            }
        }


        private void AddMenuItem(EditorMenuItemAttribute attr,MethodInfo m)
        {
            if (string.IsNullOrEmpty(attr.Category) || string.IsNullOrEmpty(attr.MenuPath)) return;

            var list = m_menulist.FirstOrDefault((x) => { return x.Label == attr.Category; });

            if (list == null)
            {
                list = new GUIMenuList(attr.Category);
                SetListOrder(list);
                m_menulist.Add(list);
            }
            list.AddMenuItem(attr.MenuPath, () => { m.Invoke(null,null); });
        }

    }

    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true,Inherited =false)]
    public class EditorMenuItemAttribute : Attribute
    {
        public string MenuPath;
        public string Category;
        public int Order;
        public EditorMenuItemAttribute(string category,string path,int order = 0)
        {
            MenuPath = path;
            Category = category;
            Order = order;
        }
    }

    public static class EditorMenuItemOrder
    {
        
    }
}
