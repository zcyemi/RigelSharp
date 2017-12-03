using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelEditor.EGUI
{
    public class GUISamples :GUIDockContentBase
    {
        [EditorMenuItem("Help","GUISamples")]
        public static void ShowSample()
        {
            var samples = new GUISamples();

            RigelEditorApp.Instance.EditorGUI.DockManager.AddNewContent(samples);
        }

        private readonly List<string> m_tabnames = new List<string>()
        {
            "Tab1",
            "Tab2",
            "Tab3",
        };
        private int m_tabindex = 0;
        

        public override void OnGUI()
        {
            GUI.DrawText(new Vector4(0, 0, 100, 100), "GUISamples", RigelColor.White);

            GUILayout.BeginHorizontal();

            m_tabindex = GUILayout.TabView(m_tabindex, m_tabnames, (index)=> {
                GUILayout.Button("Button " + index);
            },
            GUIOption.Width(300),GUIOption.Height(300));


            m_tabindex = GUILayout.TabViewVertical(m_tabindex, m_tabnames, (index) => {
                GUILayout.Button("ButtonV " + index);
            },50,
            GUIOption.Width(300), GUIOption.Height(300));

            GUILayout.EndHorizontal();

        }
    }
}
