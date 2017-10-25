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
        public string WindowTitle { get; protected set; }

        internal RigelEGUIWindowBufferInfo m_bufferInfo;
        
        internal int m_order = 0;

        public Vector4 m_debugColor;


        private bool m_focused = false;
        private bool m_onWindowMoveDrag = false;


        public RigelEGUIWindow()
        {
            WindowTitle = this.GetType().ToString();

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
            CheckWindowMoveDrag();

            RigelUtility.Log("draw window:" + WindowTitle + " " + m_focused);
            RigelEGUI.DrawRect(new Vector4(Position, Size.X, Size.Y), m_focused ? RigelEGUIStyle .Current.WinBGColorFocused: RigelEGUIStyle.Current.WinBGColor);
            RigelEGUI.DrawRect(new Vector4(Position, Size.X, RigelEGUIStyle.Current.WinHeaderHeight), RigelEGUIStyle.Current.WinHeaderColor);

            
        }

        void CheckWindowMoveDrag()
        {
            if (RigelEGUI.Event.Used) return;
            if(RigelEGUI.Event.EventType == RigelEGUIEventType.MouseDragEnter)
            {
                if (RigelEGUI.RectContainsCheck(Position,new Vector2(Size.X,RigelEGUIStyle.Current.WinHeaderHeight), RigelEGUI.Event.Pointer))
                {
                    m_onWindowMoveDrag = true;
                    RigelEGUI.Event.Use();
                }
            }
            else if(RigelEGUI.Event.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if (m_onWindowMoveDrag)
                {
                    Position += RigelEGUI.Event.DragOffset;
                    RigelEGUI.Event.Use();
                }
            }
            else if(RigelEGUI.Event.EventType == RigelEGUIEventType.MouseDragLeave)
            {
                if (m_onWindowMoveDrag)
                {
                    m_onWindowMoveDrag = false;
                    RigelEGUI.Event.Use();
                }
            }
        }
    }
}
