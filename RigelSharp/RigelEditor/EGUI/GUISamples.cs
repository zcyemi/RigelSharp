using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelEditor.EGUI
{
    public class GUISamples : GUIDockContentBase
    {
        [EditorMenuItem("Help", "GUISamples")]
        public static void ShowSample()
        {
            var samples = new GUISamples();

            RigelEditorApp.Instance.EditorGUI.DockManager.AddNewContent(samples);
        }

        private readonly List<string> m_tabnames = new List<string>()
        {
            "Index",        //0
            "TabView",      //1
            "Text",         //2
            "ScrollView",   //3
            "Container",    //4
            "Button",       //5
            "Styles",       //6
            "Layout",       //7
            "DragDrop",     //8
            "Dialogs",      //9
            "Widget",       //10
            "Geometry",     //11
        };
        private int m_sampleindex = 0;


        public override void OnGUI()
        {
            m_sampleindex = GUILayout.TabViewVertical(m_sampleindex, m_tabnames, (index) =>
            {
                switch (index)
                {
                    case 0:
                        SampleIndexPage();
                        break;
                    case 1:
                        SampleTabView();
                        break;
                    case 2:
                        SampleText();
                        break;
                    case 4:
                        SampleContainer();
                        break;
                    case 5:
                        SampleButton();
                        break;
                    case 7:
                        SampleLayout();
                        break;
                    case 8:
                        SampleDragDrop();
                        break;
                    case 10:
                        SampleWidget();
                        break;
                    case 11:
                        SampleGeometry();
                        break;
                }
            }, 75);


        }

        #region IndexPage
        private void SampleIndexPage()
        {
            GUILayout.Text("EGUI Samples");
        }
        #endregion

        #region TabView
        private int m_sampleTabIndex = 0;
        private List<string> m_sampleTabNames = new List<string>()
        {
            "Tab1",
            "Tab2",
            "Tab3"
        };
        private void SampleTabView()
        {
            GUILayout.Indent(10);
            GUILayout.Text("TabView Horizontal");
            m_sampleTabIndex = GUILayout.TabView(m_sampleTabIndex, m_sampleTabNames, (index) =>
            {
                GUILayout.Button("TabItems " + index);
            }, GUIOption.Width(300), GUIOption.Height(100));

            GUILayout.Space(10);

            GUILayout.Text("TabView Vertical");
            m_sampleTabIndex = GUILayout.TabViewVertical(m_sampleTabIndex, m_sampleTabNames, (index) =>
            {
                GUILayout.Button("TabItems " + index);
            }, 50, GUIOption.Width(300), GUIOption.Height(100));
        }
        #endregion

        #region Text
        private int m_sampleTextTabIndex = 0;
        private List<string> m_sampleTextTabList = new List<string>() { "Text", "TextBlock" };
        private string m_sampleLongText = "A game engine is a software framework designed for the creation and development of video games.Developers use them to create games for consoles, mobile devices and personal computers.The core functionality typically provided by a game engine includes a rendering engine(renderer) for 2D or 3D graphics, a physics engine or collision detection(and collision response), sound, scripting";
        private void SampleText()
        {
            m_sampleTextTabIndex = GUILayout.TabView(m_sampleTextTabIndex, m_sampleTextTabList, (index) =>
            {
                if(index == 0)
                {
                    //GUI
                    GUILayout.Text("GUI.DrawChar");
                    var rect = GUI.GetRectAbsolute(new Vector4(5, 30, 20, 20));
                    GUI.RectA(rect, RigelColor.White);
                    GUI.TextA(rect, new Vector2(-3, -3), "R", RigelColor.Black);
                    rect.X += 25;
                    GUI.RectA(rect, RigelColor.White);
                    GUI.TextA(rect, new Vector2(13, 10), "R", RigelColor.Black);

                    GUILayout.Space(30);

                    GUILayout.Text("GUI.DrawTextA");

                    rect = new Vector4(rect.X - 25, rect.Y + 50, 50, 30);
                    GUI.RectA(rect, RigelColor.White);
                    GUI.TextA(rect, new Vector2(-5, -3), "ABCDEFGHI", RigelColor.Black);
                    GUI.TextA(rect, new Vector2(5, 20), "ZXVBNMGHGJ", RigelColor.Black);

                    GUILayout.Space(35);

                    GUILayout.Text("GUI.DrawText (With Group)");
                    GUI.BeginGroup(new Vector4(5, 140, 135, 50), RigelColor.White);
                    {
                        GUI.Text(new Vector4(-10, -3, 140, 30), "HELLOWORLD!", GUIStyle.Current.ColorActive);
                        var rectg = new Vector4(50, 20, 100, 25);
                        GUI.Rect(rectg, RigelColor.Black);
                        GUI.Text(rectg, "HELLO WORLD!", GUIStyle.Current.ColorActive);
                    }
                    GUI.EndGroup();

                    //GUILayout
                    GUILayout.Space(55);
                    GUILayout.Text("GUILayout Text");
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Text("DefaultText");
                        GUILayout.Text("Text with Border", null, null, 3, GUIOption.Border(GUIStyle.Current.ColorActiveD));
                        GUILayout.Text("TextWithColor", GUIStyle.Current.ColorActiveD);
                        GUILayout.Text("TextWithBG", GUI.Context.Color, GUIStyle.Current.ColorActiveD);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Text("Text Width");
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Text("100px", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100));
                        GUILayout.Text("50px", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(50));
                        GUILayout.Text("20px", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(10));
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Text("Horizontal Alignment");
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Text("Align Center", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignHCenter);
                        GUILayout.Text("Align Center", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignHLeft);
                        GUILayout.Text("Align Right", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignHRight);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Text("Vertical Alignment");
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.SetLineHeight(30);
                        GUILayout.Text("Line Height 30", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignHCenter);

                        GUILayout.Text("Align Top", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignVTop);
                        GUILayout.Text("Align Center", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignVCenter);
                        GUILayout.Text("Align Bottom", GUI.Context.Color, GUIStyle.Current.ColorActiveD, GUIOption.Width(100), GUIOption.AlignVBottom);

                        GUILayout.RestoreLineHeight();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.TextBlock(m_sampleLongText);
                }
            });
            

        }
        #endregion

        #region Container

        private void SampleContainer()
        {
            GUILayout.Text("GUI.Group");
            {
                GUI.BeginGroup(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100), GUIStyle.Current.BackgroundColorS1);
                {
                    //Rect clipped
                    GUI.Rect(new Vector4(-10, -20, 30, 30), RigelColor.Red);

                    //Text clipped
                    GUI.Text(new Vector4(50, 30, 100, 30), "Text is clipped by the group rect.");
                }
                GUI.EndGroup();

                GUILayout.Space(100);
            }



            GUILayout.Text("GUILayout.Area");
            {
                //Area Absolutely
                GUILayout.BeginArea(GUILayout.GetRectAbsolute(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100)), GUIStyle.Current.BackgroundColorS1);
                GUILayout.Text("Absolutely");
                GUILayout.EndArea();

                ////Area Relative
                GUILayout.BeginAreaR(new Vector4(105, GUILayout.CurrentLayout.Offset.Y, 100, 100), GUIStyle.Current.BackgroundColorS1);
                GUILayout.Text("Relative");
                GUILayout.EndArea();

                GUILayout.Space(100);
            }

            GUILayout.Text("GUILayout.Container");
            {
                var rect = new Vector4(GUILayout.CurrentLayout.Offset, 100, 100);
                rect.X += 100;
                var rectab = GUILayout.GetRectAbsolute(rect);

                //container synchronize the rect of GUI.Group and GUILayout.Area

                //Container Absolutely
                GUILayout.BeginContainer(rectab, RigelColor.Green);
                GUI.Rect(new Vector4(50, 50, 50, 50), RigelColor.Blue);
                GUILayout.Rect(new Vector4(25, 25, 25, 25), RigelColor.Red);
                GUILayout.EndContainer();

                //Container Relative
                GUILayout.BeginContainerRelative(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100), RigelColor.Red);
                GUILayout.Rect(new Vector4(25, 25, 25, 25), RigelColor.White);
                GUI.Rect(new Vector4(50, 50, 50, 50), RigelColor.Black);
                GUILayout.EndContainer();
                GUILayout.Space(100);
            }

        }

        #endregion

        #region Button
        private void SampleButton()
        {
            var recta = new Vector4(GUI.Context.currentGroup.Absolute.Pos(),50,20);
            GUI.ButtonA(recta, "ButtonA");
            recta.X += 55;
            recta.Z = 100;
            GUI.ButtonA(recta, "ButtonA");
        }
        #endregion

        #region Layout

        [TODO("Bug", "Wrong flow caculation!")]
        private void SampleLayout()
        {
            GUILayout.Text("GUILayout Flow");
            GUI.Context.BackgroundColor.Set(GUIStyle.Current.BackgroundColorS1);

            GUILayout.Text("GUILayout.Horizontal");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Button("V1-0");
                        GUILayout.Button("V1-1");
                        GUILayout.Button("V1-2");
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Button("V0-0");
                        GUILayout.Button("V0-1");
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Button("V1-H0-0");
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Button("V1-H1-0");
                            GUILayout.Button("V1-H1-1");
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                }
                GUILayout.EndHorizontal();
                //wrong
                GUILayout.Text("AAAAAA");
            }

            GUILayout.Text("GUILayout.Vertical");

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Text("TestText");
                    GUILayout.BeginVertical();
                    GUILayout.Button("ButtonExtend");
                    GUILayout.Button("ButtonExtend");
                    GUILayout.EndVertical();
                    GUILayout.Button("TestButton");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Button("Width 100px", GUIOption.Width(100));
                    GUILayout.Button("Height 40px", GUIOption.Height(40));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Text("Test Text");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();


            GUI.Context.BackgroundColor.Restore();
        }
        #endregion

        #region DragDrop
        private void SampleDragDrop()
        {

        }
        #endregion

        #region Widget
        private void SampleWidget()
        {
            GUILayout.Text("Slider");

            GUILayout.Text("Toggle");

            GUILayout.Text("Popup");


        }
        #endregion

        #region Geometry
        private void SampleGeometry()
        {
            GUILayout.Text("Rect");
            GUI.Rect(new Vector4(5, 25, 20, 10), GUIStyle.Current.ColorActiveD);

            var rect = new Vector4(GUI.Context.currentGroup.Absolute.Pos(), 20, 10);
            rect.X += 30;
            rect.Y += 25;
            GUI.RectA(rect, GUIStyle.Current.ColorActiveD);
        }
#endregion
    }
}
