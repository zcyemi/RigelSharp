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


    public class GUITestContent: GUIDockContentBase
    {
        private bool m_collapsegroup1 = false;
        private bool m_collapsegroup2 = false;
        private Vector2 m_scrollViewPos = Vector2.Zero;

        public GUITestContent()
        {
            Title = "GUITest";
        }

        public override void OnGUI()
        {

            GUILayout.Text(GUI.DrawTarget.bufferText.Count.ToString());
            GUILayout.Line(2,null);

            GUILayout.Text("GUITest Component");

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
            

            GUILayout.Line(1,null);

            {
                GUILayout.BeginHorizontal();
                GUILayout.Text("MenuBar");
                GUI.DrawBorder(GUILayout.s_ctx.currentLayout.LastRect, 1, RigelColor.Red);

                GUILayout.EndHorizontal();
            }

            {
                //Options
                GUILayout.Button("Expended", GUIOption.Expended);
                GUILayout.BeginHorizontal();
                GUILayout.Button("Grid 0.5", GUIOption.Grid(0.5f));
                GUILayout.Button("Grid 0.25", GUIOption.Grid(0.25f));

                GUILayout.EndHorizontal();
            }


            {
                //CollapseGroup
                if(GUILayout.BeginCollapseGroup("CollapseGroup-1",ref m_collapsegroup1))
                {
                    GUILayout.Text("text in collapse group.");
                }
                GUILayout.EndCollapseGroup();

                m_collapsegroup2 = GUILayout.BeginCollapseGroup("CollapseGroup-2", ref m_collapsegroup2);
                if (m_collapsegroup2)
                {
                    GUILayout.Button("button in collapse group2.");
                }

                var t = GUI.Depth;
                GUILayout.EndCollapseGroup();
            }

            {
                //ScrollView

                m_scrollViewPos = GUILayout.BeginScrollView(m_scrollViewPos);


                GUILayout.Button("Test Button");

                GUILayout.Text("Test Text");

                GUILayout.EndScrollView();
            }

        }
    }
}
