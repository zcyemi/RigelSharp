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

        public int BufRectSize { get { return BufferRectEndPos - BufferRectStartPos; } }
        public int BUfTextSize { get { return BufferTextEndPos - BufferTextStartPos; } }
    }

    public class RigelEGUIWindow
    {

        public Vector2 Position = new Vector2(20, 20);
        public Vector2 Size = new Vector2(400, 300);
        public bool Focused { get { return m_focused; } internal set { m_focused = value; } }
        public int Order { get { return m_order; } }

        internal RigelEGUIWindowBufferInfo m_bufferInfo;
        private bool m_focused = false;
        internal int m_order = 0;


        public RigelEGUIWindow()
        {
            m_bufferInfo.BufferInited = false;
            m_bufferInfo.BufferRectEndPos = 0;
            m_bufferInfo.BufferRectStartPos = 0;
            m_bufferInfo.BufferTextEndPos = 0;
            m_bufferInfo.BufferTextStartPos = 0;

            OnStart();
        }

        public virtual void OnStart()
        {

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

        internal void InternalDrawBasis()
        {
            RigelEGUI.DrawRect(new Vector4(Position, Size.X, Size.Y), m_focused ? RigelEGUIStyle.Current.WinBGColorFocused : RigelEGUIStyle.Current.WinBGColor);
        }
    }
}
