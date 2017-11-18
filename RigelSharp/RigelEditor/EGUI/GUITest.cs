using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUITestComponent : IGUIComponent
    {
        public override void Draw(GUIEvent guievent)
        {
            GUILayout.BeginArea(new Vector4(50, 50, 400, 300));
            GUILayout.Button("Button1");
            if (GUILayout.Button("Button2"))
            {
                Distroy = true;
            }
            GUILayout.EndArea();
        }

    }


    public class GUITestLayout
    {
        public static void Sample_GroupMixedWithArea()
        {
            var narea = GUI.Context.currentArea;
            narea.Y = 20;
            narea.W -= 20;

            var ngroup = GUI.Context.currentGroup.Rect;
            ngroup.Y = 20;
            ngroup.W -= 20;
            GUI.BeginGroup(ngroup, RigelColor.Random());
            GUILayout.BeginArea(narea);

            GUILayout.Button("BtnArea");

            GUI.Button(new Vector4(100, 0, 100, 20), "BtnGroup");

            GUI.BeginGroup(new Vector4(100, 100, 100, 100), RigelColor.Random());

            GUILayout.Button("BtnAra2");
            GUI.Button(new Vector4(0, 0, 100, 20), "BtnGroup2");
            GUI.EndGroup();

            GUILayout.BeginArea(new Vector4(200, 100, 100, 200), RigelColor.Random());
            GUILayout.BeginToolBar(20);
            GUILayout.Text("Thisisatoolbar");
            GUILayout.EndToolBar();

            GUILayout.EndArea();



            GUILayout.EndArea();
            GUI.EndGroup();
        }
    }


    public class GUITestContent : GUIDockContentBase
    {
        private bool m_collapsegroup1 = false;
        private bool m_collapsegroup2 = false;
        private Vector2 m_scrollViewPos = Vector2.Zero;

        private bool m_sampleGUIOption= false;
        private bool m_sampleScrollView = false;
        private bool m_sampleGUIComponent = false;

        public GUITestContent()
        {
            Title = "GUITest";
        }

        public override void OnGUI()
        {

            GUILayout.Text(GUI.DrawTarget.bufferText.Count.ToString());
            GUILayout.Line(2, null);

            GUILayout.Line(1, null);

            {
                GUILayout.BeginHorizontal();
                GUILayout.Text("MenuBar");
                GUI.DrawBorder(GUILayout.s_ctx.currentLayout.LastRect, 1, RigelColor.Red);

                GUILayout.EndHorizontal();
            }

            SampleGUIComponent();

            SampleGUIOption();
            SampleScrollView();

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
