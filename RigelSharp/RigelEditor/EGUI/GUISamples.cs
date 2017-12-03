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
            "Index",
            "TabView",
            "Text",
            "ScrollView",
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
    }
}
