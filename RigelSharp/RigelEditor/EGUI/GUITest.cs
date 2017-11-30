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

        [TODO("Bug", "Can't Add GUITestContent again after delete from docknode.")]
        [EditorMenuItem("Help", "GUITestWindow")]
        public static void ShowTestWindow()
        {
            if (s_content == null)
            {
                s_content = new GUITestContent();

                var dockmgr = RigelEditorApp.Instance.EditorGUI.DockManager;
                if (dockmgr.FindDockContent<GUITestContent>() == null)
                {
                    dockmgr.AddNewContent(s_content);
                }

            }
        }



        private bool m_sampleGUIOption = false;
        private bool m_sampleScrollView = false;
        private bool m_sampleGUIComponent = false;
        private bool m_sampleText = false;

        private bool m_sampleInput = false;
        private bool m_sampleDialog = false;
        private bool m_sampleLayout = false;
        private bool m_sampleDrawChar = false;

        public GUITestContent()
        {
            Title = "GUITest";
        }

        public override void OnGUI()
        {
            SampleDrawChar();

            SampleLayouting();

            SampleGUIComponent();
            SampleGUIOption();
            SampleText();
            SampleScrollView();
            SampleInput();
            SampleDialog();

        }

        private void SampleDrawChar()
        {
            var rect = new Vector4(20, 500, 20, 20);

            GUI.DrawRect(rect,RigelColor.Black,true);
            GUI.DrawCharWithRect(rect, new Vector2(-3,-3), 'R', RigelColor.White);

            rect.X += 21;
            rect.Z = 10;
            GUI.DrawRect(rect, RigelColor.Black, true);
            GUI.DrawCharWithRect(rect, new Vector2(0,10), 'W', RigelColor.White);

            rect.Y += 20;
            rect.Z = 30;
            rect.W = 20;
            GUI.DrawTextA(rect,new Vector2(-5,0), "ABCDEFGHOLS", GUI.Context.Color);

            GUI.BeginGroup(new Vector4(200, 250, 50, 50), RigelColor.Black, false);

            GUI.DrawText(new Vector4(-25, -7, 50, 30), "QWERTYUIOP", RigelColor.White);
            GUI.EndGroup();
        }

        private void SampleLayouting()
        {
            if (GUILayout.BeginCollapseGroup("Layout", ref m_sampleLayout))
            {
                GUI.BeginGroup(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100));

                GUI.DrawRect(new Vector4(0, 0, 20, 20), RigelColor.Red, false);

                GUI.EndGroup();

                var rect = new Vector4(GUILayout.CurrentLayout.Offset, 100, 100);
                rect.X += 100;
                var rectab = GUILayout.GetRectAbsolute(rect);
                GUILayout.BeginContainer(rectab, RigelColor.Green);

                GUI.DrawRect(new Vector4(50, 50, 50, 50), RigelColor.Blue);
                GUILayout.DrawRect(new Vector4(25, 25, 25, 25), RigelColor.Red);

                GUILayout.EndContainer();

                GUILayout.BeginContainerRelative(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100), RigelColor.Red);

                GUILayout.DrawRect(new Vector4(25, 25, 25, 25), RigelColor.White);
                GUI.DrawRect(new Vector4(50, 50, 50, 50), RigelColor.Black);
                GUILayout.EndContainer();
                GUILayout.Space(200);
            }
            GUILayout.EndCollapseGroup();
        }

        private string m_inputString = "sampleString";
        private string m_inputString2 = "sampleString2";
        private void SampleInput()
        {
            if (GUILayout.BeginCollapseGroup("Input", ref m_sampleInput))
            {
                GUILayout.Button("Test");
                m_inputString = GUILayout.TextInput("Input", m_inputString);
                m_inputString2 = GUILayout.TextInput("Sample String 2", m_inputString2);
            }
            GUILayout.EndCollapseGroup();
        }

        private void SampleDialog()
        {
            if (GUILayout.BeginCollapseGroup("Dialogs", ref m_sampleDialog))
            {
                if (GUILayout.Button("EditorTestWindowedDialog"))
                {
                    var dialog = new EditorTestWindowedDialog();
                    GUI.DrawComponent(dialog);
                }
            }
            GUILayout.EndCollapseGroup();
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
        private void SampleText()
        {
            string longtext = "A game engine is a software framework designed for the creation and development of video games.Developers use them to create games for consoles, mobile devices and personal computers.The core functionality typically provided by a game engine includes a rendering engine(renderer) for 2D or 3D graphics, a physics engine or collision detection(and collision response), sound, scripting";

            if (GUILayout.BeginCollapseGroup("[TextRendering]", ref m_sampleText))
            {
                //GUILayout.Text(longtext);
                //GUILayout.Text(@"Single line text.");
                //GUILayout.Text("Space[ ]");
                //GUILayout.Text("Tab[\t]");
                GUILayout.BeginContainer(new Vector4(GUILayout.s_ctx.currentLayout.Offset, 100, 100), RigelColor.Green);

                GUI.TextBlock(new Vector4(0, 0, 100, 100), longtext, RigelColor.Red);
                GUILayout.EndContainer();
                GUILayout.Space(100);

                GUILayout.TextBlock(longtext, GUIOption.Width(300));
                GUILayout.TextBlock(longtext);
            }
            GUILayout.EndCollapseGroup();
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

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Button("AlignLeft", GUIOption.AlignHLeft, GUIOption.Grid(0.33f));
                    GUILayout.Button("AlignCenter", GUIOption.AlignHCenter, GUIOption.Grid(0.34f));
                    GUILayout.Button("AlignRight", GUIOption.AlignHRight, GUIOption.Grid(0.33f));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndCollapseGroup();
        }


        private Vector2 m_scrollViewPosV = Vector2.Zero;
        private Vector2 m_scrollViewPosH = Vector2.Zero;
        private Vector2 m_scrollViewPosAll = Vector2.Zero;
        private void SampleScrollView()
        {
            if (GUILayout.BeginCollapseGroup("[ScrollView]", ref m_sampleScrollView))
            {
                //scrollview1
                GUILayout.BeginHorizontal();
                {
                    m_scrollViewPosV = GUILayout.BeginScrollView(m_scrollViewPosV, GUIScrollType.Vertical, GUIOption.Width(200), GUIOption.Height(200));
                    {
                        GUILayout.Button("Scroll Vertical");
                        for (int i = 0; i < 5; i++)
                        {
                            GUILayout.Space(100);
                            GUILayout.Text("WWW" + i);
                            GUILayout.Button("Btn" + i);
                        }
                    }
                    GUILayout.EndScrollView();

                    m_scrollViewPosH = GUILayout.BeginScrollView(m_scrollViewPosH, GUIScrollType.Horizontal, GUIOption.Width(200), GUIOption.Height(200));
                    {
                        GUILayout.Button("Scroll Horizontal");

                        GUILayout.Space(20);
                        GUILayout.BeginHorizontal();
                        for (int i = 0; i < 5; i++)
                        {
                            GUILayout.Button("Btn " + i);
                            GUILayout.Text("TEXT:" + i);
                            GUILayout.Indent(100);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        for (int i = 0; i < 7; i++)
                        {
                            GUILayout.Button("-Btn----" + i);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();

                    m_scrollViewPosAll = GUILayout.BeginScrollView(m_scrollViewPosAll, GUIScrollType.All, GUIOption.Width(200), GUIOption.Height(200));
                    {
                        GUILayout.Button("Scroll All");

                        GUILayout.BeginHorizontal();
                        for (int i = 0; i < 7; i++)
                        {
                            GUILayout.Button("-Btn----" + i);
                        }
                        GUILayout.EndHorizontal();


                        GUILayout.BeginVertical();

                        for (int i = 0; i < 5; i++)
                        {
                            GUILayout.Space(100);
                            GUILayout.Text("WWW" + i);
                            GUILayout.Button("Btn" + i);
                        }

                        GUILayout.EndVertical();

                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndHorizontal();

                GUILayout.Button("dwdw");
            }
            GUILayout.EndCollapseGroup();
        }
    }
}
