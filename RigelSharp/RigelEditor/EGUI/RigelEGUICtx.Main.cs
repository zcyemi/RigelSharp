using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public partial class RigelEGUICtx : IDisposable
    {
        private RigelEGUIDockerManager m_dockerMgr = null;
        private RigelEGUIMenu m_mainMenu = null;

        private List<RigelEGUIVertex> m_bufMainRect = new List<RigelEGUIVertex>();
        private List<RigelEGUIVertex> m_bufMainText = new List<RigelEGUIVertex>();

        internal List<RigelEGUIVertex> BufMainRect { get { return m_bufMainRect; } }
        internal List<RigelEGUIVertex> BufMainText { get { return m_bufMainText; } }

        private bool m_bufMainRectEmptyBlock = false;
        private bool m_bufMainTextEmptyBlock = false;

        private Vector4 m_mainStatusBarRect;


        private void GUIUpdateMainBegin(RigelEGUIEvent guievent)
        {
            m_mainStatusBarRect.Y = ClientHeight - 20;
            m_mainStatusBarRect.Z = ClientWidth;
            m_mainStatusBarRect.W = 20;

            m_bufMainRectEmptyBlock = false;
            m_bufMainTextEmptyBlock = false;

            BufMainRect.Clear();
            BufMainText.Clear();

            RigelEGUILayout.Frame(ClientWidth,ClientHeight);

            GUIMainDrawMenuBar();
        }

        private void GUIUpdateMainEnd(RigelEGUIEvent guievent)
        {
            m_graphicsBind.BufferMainRect.CheckAndExtendsWithSize(BufMainRect.Count);
            BufMainRect.CopyTo(m_graphicsBind.BufferMainRect.BufferData);
            m_graphicsBind.BufferMainRect.InternalSetBufferDataCount(BufMainRect.Count);


            m_graphicsBind.BufferMainText.CheckAndExtendsWithSize(BufMainText.Count);
            BufMainText.CopyTo(m_graphicsBind.BufferMainText.BufferData);
            m_graphicsBind.BufferMainText.InternalSetBufferDataCount(BufMainText.Count);
        }

        private void GUIMainDrawMenuBar()
        {
            //menu
            //RigelEGUI.DrawRect(new Vector4(0, 0, ClientWidth, 25), RigelEGUIStyle.Current.MainMenuBGColor);

            RigelEGUILayout.BeginMenuBar();

            RigelEGUILayout.Button("Test");
            RigelEGUILayout.Indent(1);
            RigelEGUILayout.Button("Test2");

            RigelEGUILayout.EndMenuBar();

            RigelEGUILayout.BeginArea(new Vector4(100, 20, 100, 100));
            RigelEGUI.DrawRect(new Vector4(0, 0, 100, 100),RigelColor.White);
            RigelEGUILayout.EndArea();


            //RigelEGUILayoutTest.LayoutButtonTab();
            //RigelEGUILayoutTest.LayoutTextSample1();
            //RigelEGUILayoutTest.LayoutBasic();
            //RigelEGUILayoutTest.LayoutAreaSample();

            //status bar
            RigelEGUI.DrawRect(m_mainStatusBarRect, GUIStyle.Current.MainStatusBarColor);
        }
    }
}
