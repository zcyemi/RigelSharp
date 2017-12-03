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

        };
        private int m_sampleindex = 0;


        public override void OnGUI()
        {
            m_sampleindex = GUILayout.TabViewVertical(m_sampleindex, m_tabnames, (index) => {
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
                    case 7:
                        SampleLayout();
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
            },GUIOption.Width(300), GUIOption.Height(100));

            GUILayout.Space(10);

            GUILayout.Text("TabView Vertical");
            m_sampleTabIndex = GUILayout.TabViewVertical(m_sampleTabIndex, m_sampleTabNames, (index) =>
            {
                GUILayout.Button("TabItems " + index);
            },50, GUIOption.Width(300), GUIOption.Height(100));
        }
        #endregion

        #region Text
        private void SampleText()
        {
            //GUI
            GUILayout.Text("GUI.DrawChar");
            var rect = GUI.GetRectAbsolute(new Vector4(5, 30, 20, 20));
            GUI.DrawRect(rect, RigelColor.White,true);
            GUI.TextA(rect, new Vector2(-3, -3), "R", RigelColor.Black);
            rect.X += 25;
            GUI.DrawRect(rect, RigelColor.White, true);
            GUI.TextA(rect, new Vector2(13, 10), "R", RigelColor.Black);

            GUILayout.Space(30);

            GUILayout.Text("GUI.DrawTextA");

            rect = new Vector4(rect.X - 25, rect.Y + 50, 50, 30);
            GUI.DrawRect(rect, RigelColor.White, true);
            GUI.TextA(rect, new Vector2(-5, -3), "ABCDEFGHI",RigelColor.Black);
            GUI.TextA(rect, new Vector2(5, 20), "ZXVBNMGHGJ", RigelColor.Black);

            GUILayout.Space(35);

            GUILayout.Text("GUI.DrawText (With Group)");
            GUI.BeginGroup(new Vector4(5, 140, 135, 50), RigelColor.White);
            {
                GUI.Text(new Vector4(-10, -3, 140, 30), "HELLOWORLD!", GUIStyle.Current.ColorActive);
                var rectg = new Vector4(50, 20, 100, 25);
                GUI.DrawRect(rectg, RigelColor.Black);
                GUI.Text(rectg, "HELLO WORLD!", GUIStyle.Current.ColorActive);
            }
            GUI.EndGroup();

            //GUILayout
            GUILayout.Space(55);
            GUILayout.Text("GUILayout Text");
            GUILayout.BeginHorizontal();
            {
                GUILayout.TextN("DefaultText");
                GUILayout.TextN("Text with Border", null, null, 3, GUIOption.Border(GUIStyle.Current.ColorActiveD));
                GUILayout.TextN(GUIStyle.Current.ColorActiveD, "TextWithColor");
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "TextWithBG");
            }
            GUILayout.EndHorizontal();

            GUILayout.Text("Text Width");
            GUILayout.BeginHorizontal();
            {
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "100px", GUIOption.Width(100));
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "50px", GUIOption.Width(50));
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "20px", GUIOption.Width(10));
            }
            GUILayout.EndHorizontal();

            GUILayout.Text("Horizontal Alignment");
            GUILayout.BeginHorizontal();
            {
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Center", GUIOption.Width(100),GUIOption.AlignHCenter);
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Left", GUIOption.Width(100), GUIOption.AlignHLeft);
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Right", GUIOption.Width(100), GUIOption.AlignHRight);
            }
            GUILayout.EndHorizontal();

            GUILayout.Text("Vertical Alignment");
            GUILayout.BeginHorizontal();
            {
                GUILayout.SetLineHeight(30);
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Line Height 30", GUIOption.Width(100), GUIOption.AlignHCenter);

                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Top", GUIOption.Width(100), GUIOption.AlignVTop);
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Center", GUIOption.Width(100), GUIOption.AlignVCenter);
                GUILayout.TextN(GUI.Context.Color, GUIStyle.Current.ColorActiveD, "Align Bottom", GUIOption.Width(100), GUIOption.AlignVBottom);

                GUILayout.RestoreLineHeight();
            }
            GUILayout.EndHorizontal();

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
                    GUI.DrawRect(new Vector4(-10, -20,30, 30), RigelColor.Red, false);

                    //Text clipped
                    GUI.Text(new Vector4(50, 30, 100, 30), "Text is clipped by the group rect.");
                }
                GUI.EndGroup();

                GUILayout.Space(100);
            }
            


            GUILayout.Text("GUILayout.Area");
            {
                //Area Absolutely
                GUILayout.BeginArea(GUILayout.GetRectAbsolute(new Vector4(GUILayout.CurrentLayout.Offset,100,100)), GUIStyle.Current.BackgroundColorS1);
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
                GUI.DrawRect(new Vector4(50, 50, 50, 50), RigelColor.Blue);
                GUILayout.DrawRect(new Vector4(25, 25, 25, 25), RigelColor.Red);
                GUILayout.EndContainer();

                //Container Relative
                GUILayout.BeginContainerRelative(new Vector4(GUILayout.CurrentLayout.Offset, 100, 100), RigelColor.Red);
                GUILayout.DrawRect(new Vector4(25, 25, 25, 25), RigelColor.White);
                GUI.DrawRect(new Vector4(50, 50, 50, 50), RigelColor.Black);
                GUILayout.EndContainer();
                GUILayout.Space(100);
            }
            
        }

        #endregion

        #region Layout

        [TODO("Bug","Wrong flow caculation!")]
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
                    GUILayout.TextN("TestText");
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
    }
}
