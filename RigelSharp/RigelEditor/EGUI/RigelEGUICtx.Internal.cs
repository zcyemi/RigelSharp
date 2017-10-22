using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RigelEditor.EGUI
{
    public partial class RigelEGUICtx
    {
        private RigelEditorGUIDockerManager m_dockerMgr;
        private RigelEGUIMenu m_mainMenu;
        private List<RigelEditorGUIWindow> m_windows;

        private void GUIInit()
        {
            m_windows = new List<RigelEditorGUIWindow>();

            m_mainMenu = new RigelEGUIMenu();
            RefreshMainMenu();

            m_dockerMgr = new RigelEditorGUIDockerManager();
        }

        private void GUIRelease()
        {
            m_windows.Clear();
            m_mainMenu.ClearAllItems();
        }

        private void GUIUpdate()
        {
            //draw menu

            foreach (var win in m_windows)
            {
                UpdateWindow(win);
            }
        }

        private void RefreshMainMenu()
        {
            var assembly = RigelReflectionHelper.AssemblyRigelEditor;
            foreach (var type in assembly.GetTypes())
            {
                var methods = RigelReflectionHelper.GetMethodByAttribute<RigelEGUIMenuItemAttribute>(type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var m in methods)
                {
                    var attr = Attribute.GetCustomAttribute(m, typeof(RigelEGUIMenuItemAttribute)) as RigelEGUIMenuItemAttribute;
                    m_mainMenu.AddMenuItem(attr.Label, m);
                }

            }
            RigelUtility.Log("EGUI mainMenu item count:" + m_mainMenu.ItemNodes.Count());

        }

        private void UpdateWindow(RigelEditorGUIWindow win)
        {
            bool needupdate = false;
            if (win.InternalBufferInfo.FirstGenerated == false) needupdate = true;
            if (win.Focused)
            {

            }

        }
    }
}
