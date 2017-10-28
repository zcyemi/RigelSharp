using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public partial class RigelEGUICtx
    {
        private RigelEGUIDockerManager m_dockerMgr;
        private RigelEGUIMenu m_mainMenu;

        private List<RigelEGUIVertex> m_bufMainRect = new List<RigelEGUIVertex>();
        private List<RigelEGUIVertex> m_bufMainText = new List<RigelEGUIVertex>();

        internal List<RigelEGUIVertex> BufMainRect { get { return m_bufMainRect; } }
        internal List<RigelEGUIVertex> BufMainText { get { return m_bufMainText; } }

        private bool m_bufMainRectEmptyBlock = false;
        private bool m_bufMainTextEmptyBlock = false;


        private void GUIUpdateMainBegin(RigelEGUIEvent guievent)
        {
            m_bufMainRectEmptyBlock = false;
            m_bufMainTextEmptyBlock = false;

            BufMainRect.Clear();
            BufMainText.Clear();

            GUIMainDrawMenuBar();
        }

        private void GUIUpdateMainEnd(RigelEGUIEvent guievent)
        {

        }

        private void GUIMainDrawMenuBar()
        {
            RigelEGUI.DrawRect(new Vector4(0, 0, ClientWidth, 20), RigelEGUIStyle.Current.MainMenuBGColor);

        }
    }
}
