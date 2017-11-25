using SharpDX;
using System;
using System.Collections.Generic;

using RigelCore;

namespace RigelEditor.EGUI
{


    public abstract class IGUIComponent
    {
        public int BufferRectStart;
        public int BufferRectEnd;
        public int BufferTextStart;
        public int BufferTextEnd;

        public int BufferRectCount { get { return BufferRectEnd - BufferRectStart; } }
        public int BufferTextCount { get { return BufferTextEnd - BufferTextStart; } }

        public bool InitDrawed = false;
        public bool Destroy { get; internal set; }

        public void Show()
        {
            GUI.DrawComponent(this);
        }

        public void OnDestroy()
        {
            Destroy = true;
        }

        public abstract void Draw(GUIEvent guievent);
    }

    public abstract class GUIDialog:IGUIComponent
    {
        protected string m_title = "";
        protected Vector2 m_size = new Vector2(400, 300);


        public sealed override void Draw(GUIEvent guievent)
        {
            var rect = new Vector4(0, 0, m_size.X,m_size.Y).CenterPos(GUI.Context.baseRect.Size());

            GUILayout.BeginArea(rect,GUIStyle.Current.BackgroundColor,GUIOption.Border());

            GUILayout.Text(m_title);

            OnDraw();

            GUILayout.EndArea();
        }

        protected abstract void OnDraw();
    }

    public abstract class GUIWindowedDialog: IGUIComponent
    {
        protected bool m_dialogMoveable = true;
        protected bool m_dialogRezieable = true;
        protected bool m_dialogCloseable = true;

        protected Vector2 m_dialogMaxSize = new Vector2(800, 600);
        protected Vector2 m_dialogMinSize = new Vector2(625, 360);

        private Vector2 m_size;

        public GUIWindowedDialog(bool moveable,bool resizeable,bool closeable)
        {
            m_size = m_dialogMinSize;
            m_dialogMoveable = moveable;
            m_dialogRezieable = resizeable;
            m_dialogCloseable = closeable;
        }

        public sealed override void Draw(GUIEvent guievent)
        {

        }

        protected abstract void OnDraw();
    }

    public abstract class GUIOverlay: IGUIComponent
    {

        public override void Draw(GUIEvent guievent)
        {

        }
    }


    
}
