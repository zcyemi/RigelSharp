using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    internal struct RigelEGUIWindowBufferInfo
    {
        public bool BufferInited;
        public int BufferRectStartPos;
        public int BufferRectEndPos;
        public int BufferTextStartPos;
        public int BufferTextEndPos;
    }

    public class RigelEGUIWindow
    {


        public Vector4 Rect { get; private set; }
        public bool Focused { get { return m_focused; } internal set { m_focused = value; } }

        internal RigelEGUIWindowBufferInfo m_bufferInfo;
        private bool m_focused = false;


        public RigelEGUIWindow()
        {
            m_bufferInfo.BufferInited = false;
            m_bufferInfo.BufferRectEndPos = 0;
            m_bufferInfo.BufferRectStartPos = 0;
            m_bufferInfo.BufferTextEndPos = 0;
            m_bufferInfo.BufferTextStartPos = 0;
        }

        public virtual void OnMenuBar(RigelEGUIMenu menu)
        {

        }

        public virtual void OnGUI()
        {

        }

        public static T GetWindow<T>() where T : RigelEGUIWindow, new()
        {
            return RigelEGUI.s_currentCtx.FindWindowOfType<T>();
        }
    }
}
