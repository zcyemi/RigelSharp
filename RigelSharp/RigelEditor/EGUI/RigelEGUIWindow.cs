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
        public bool Inited;
        public int StartPos;
        public int EndPos;

        public int Size { get { return EndPos - StartPos; } }

    }

    public class RigelEGUIWindow
    {

        public Vector2 Position = new Vector2(20, 20);
        public Vector2 Size = new Vector2(400, 300);
        public bool Focused { get { return m_focused; } internal set { m_focused = value; } }
        public int Order { get { return m_order; } }
        public string WindowTitle { get; protected set; }

        internal RigelEGUIWindowBufferInfo BufferInfoRect = new RigelEGUIWindowBufferInfo() { Inited = false, StartPos = 0, EndPos = 0 };
        internal RigelEGUIWindowBufferInfo BufferInfoText = new RigelEGUIWindowBufferInfo() { Inited = false, StartPos = 0, EndPos = 0 };
        
        internal int m_order = 0;

        public Vector4 m_debugColor;


        private bool m_focused = false;
        private bool m_onWindowMoveDrag = false;


        public RigelEGUIWindow()
        {
            WindowTitle = this.GetType().ToString();


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
