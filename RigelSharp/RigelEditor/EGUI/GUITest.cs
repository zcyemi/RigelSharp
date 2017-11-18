using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelEditor.EGUI
{


    public class GUITestContent : GUIDockContentBase
    {
        private static GUITestContent s_content = null;

        [EditorMenuItem("Help","GUITestWindow")]
        public static void ShowTestWindow()
        {
            if(s_content == null)
            {
                s_content = new GUITestContent();

                var dockmgr = RigelEditorApp.Instance.EditorGUI.DockManager;
                dockmgr.AddNewContent(s_content);
            }

        }

        private Vector2 m_scrollViewPos = Vector2.Zero;

        private bool m_sampleGUIOption= false;
        private bool m_sampleScrollView = false;
        private bool m_sampleGUIComponent = false;
        private bool m_sampleLayout = false;

        public GUITestContent()
        {
            Title = "GUITest";
        }

        public override void OnGUI()
        {

            SampleLayout();
            SampleGUIComponent();

            SampleGUIOption();
            SampleScrollView();


        }

        private void SampleLayout()
        {

        }

        private void SampleGUIComponent()
        {
            if (GUILayout.BeginCollapseGroup("[GUIComponent]", ref m_sampleGUIComponent))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Text("MessageBox");
                if (GUILayout.Button("ShowMessageBox"))
                {
                    var msgbox = new GUIMessageBox("MsgBox", "TestInfo", () => { Console.WriteLine("Click Confirm"); }, null, "Confirm", "Cancel");
                    GUI.DrawComponent(msgbox);
                }
                if (GUILayout.Button("ShowMessageBoxWithCancel"))
                {
                    var msgbox = new GUIMessageBox("MsgBox", "Read File fails, Continue", () => { Console.WriteLine("Click OK"); }, () => { }, "OK", "Skip");
                    GUI.DrawComponent(msgbox);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndCollapseGroup();
        }

        private void SampleCollapseGroup()
        {

        }

        private void SampleGUIOption()
        {
            if (GUILayout.BeginCollapseGroup("[GUIOptions]", ref m_sampleGUIOption))
            {
                GUILayout.Button("Expended", GUIOption.Expended);
                GUILayout.BeginHorizontal();
                GUILayout.Button("Short-Label");
                GUILayout.Button("LLLLLLLLLLLLong-Label");
                GUILayout.Button("Width-100", GUIOption.Width(100));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Button("Grid 0.5", GUIOption.Grid(0.5f));
                GUILayout.Button("Grid 0.25", GUIOption.Grid(0.25f));

                GUILayout.EndHorizontal();
            }
            GUILayout.EndCollapseGroup();
        }


        private void SampleScrollView()
        {
            if (GUILayout.BeginCollapseGroup("[ScrollView]", ref m_sampleScrollView))
            {
                //scrollview1
                GUILayout.BeginHorizontal();
                m_scrollViewPos = GUILayout.BeginScrollView(m_scrollViewPos, GUIScrollType.Vertical, GUIOption.Width(200),GUIOption.Height(200));
                GUILayout.Text("SampleText");
                for (int i = 0; i < 5; i++)
                {
                    GUILayout.Space(50);
                    GUILayout.Text((i * 50).ToString());
                }
                GUILayout.EndScrollView();
                GUILayout.Button("---------");
                GUILayout.EndHorizontal();

                GUILayout.Button("dwdw");
            }
            GUILayout.EndCollapseGroup();
        }
    }
}
