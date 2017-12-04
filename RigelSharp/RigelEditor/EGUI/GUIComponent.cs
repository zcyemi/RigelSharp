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

    public abstract class GUIDialog : IGUIComponent
    {
        protected string m_title = "";
        protected Vector2 m_size = new Vector2(400, 300);


        public sealed override void Draw(GUIEvent guievent)
        {
            var rect = new Vector4(0, 0, m_size.X, m_size.Y).CenterPos(GUI.Context.baseRect.Size());

            GUILayout.BeginArea(rect, GUIStyle.Current.BackgroundColor, GUIOption.Border());

            GUILayout.Text(m_title);

            OnDraw();

            GUILayout.EndArea();
        }

        protected abstract void OnDraw();
    }

    public abstract class GUIWindowedDialog : IGUIComponent
    {
        protected bool m_dialogMoveable = true;
        protected bool m_dialogRezieable = true;
        protected bool m_dialogCloseable = true;

        protected Vector2 m_dialogMaxSize = new Vector2(800, 600);
        protected Vector2 m_dialogMinSize = new Vector2(100, 100);

        private GUIDragState m_dragMove = new GUIDragState();
        private GUIDragState m_dragResizeHV = new GUIDragState();

        private Vector2 m_size = new Vector2(400, 300);
        private Vector2 m_pos;
        private Vector4 m_hedaerRect;

        protected string m_title = "GUIWindowedDialog";

        public GUIWindowedDialog(bool moveable, bool resizeable, bool closeable)
        {
            m_dialogMoveable = moveable;
            m_dialogRezieable = resizeable;
            m_dialogCloseable = closeable;

            m_hedaerRect = new Vector4(0, 0, m_size.X, 23);

            m_pos = (GUI.Context.baseRect.Size() - m_size) * 0.5f;
        }

        public sealed override void Draw(GUIEvent guievent)
        {
            m_hedaerRect.Z = m_size.X;
            m_pos.X = (int)m_pos.X;
            m_pos.Y = (int)m_pos.Y;

            var rect = new Vector4(m_pos, m_size.X, m_size.Y);

            GUILayout.BeginContainer(rect, GUIStyle.Current.BackgroundColor, GUIOption.Border());
            {
                bool headerover = GUIUtility.RectContainsCheck(GUILayout.GetRectAbsolute(m_hedaerRect), GUI.Event.Pointer);
                GUILayout.Rect(m_hedaerRect, headerover ? GUIStyle.Current.ColorActive : GUIStyle.Current.ColorActiveD);
                GUILayout.BeginHorizontal();
                GUILayout.Text(m_title);
                if (m_dialogCloseable)
                {
                    GUILayout.Indent((int)(rect.Z - GUILayout.CurrentLayout.Offset.X - 24));
                    if (GUILayout.Button("X", GUIStyle.Current.ColorActiveD, GUIOption.Width(23)))
                    {
                        OnDestroy();
                    }
                }
                GUILayout.EndHorizontal();

                var contentrect = GUILayout.Context.currentArea.Rect;
                contentrect.Y += m_hedaerRect.W;
                contentrect.Z -= 1;
                contentrect.W -= (m_hedaerRect.W + 1);

                GUILayout.BeginContainer(contentrect);
                {
                    OnDraw();
                }
                GUILayout.EndContainer();

                //for optimize
                bool onmove = false;
                if (m_dialogMoveable && m_dragMove.OnDrag(headerover))
                {
                    m_pos += GUI.Event.DragOffset;

                    m_pos.X = MathUtil.Clamp(m_pos.X, 0, GUI.Context.baseRect.Z);
                    m_pos.Y = MathUtil.Clamp(m_pos.Y, 0, GUI.Context.baseRect.W);
                    onmove = true;
                }
                if (m_dialogRezieable && !onmove)
                {
                    var rectResize = GUILayout.Context.currentArea.Rect;
                    rectResize.Y += (rectResize.W - 3);
                    rectResize.X += rectResize.Z - 3;
                    rectResize.Z = 6;
                    rectResize.W = 6;

                    if (m_dragResizeHV.OnDrag(rectResize))
                    {
                        m_size += GUI.Event.DragOffset;
                        m_size.Y = MathUtil.Clamp(m_size.Y, m_dialogMinSize.Y, m_dialogMaxSize.Y);
                        m_size.X = MathUtil.Clamp(m_size.X, m_dialogMinSize.X, m_dialogMaxSize.X);
                    }
                }
            }

            //var grect = new Vector4(0, 0, 100, 30);
            //GUI.DrawRect(grect, RigelColor.Green);
            //GUI.DrawText(grect, "GUITEXT", RigelColor.White);
            //GUI.Button(grect, "GUIBUTTON");
            //GUI.DrawBorder(grect, 2, RigelColor.Green);

            GUILayout.EndContainer();
        }

        protected abstract void OnDraw();
    }

    public abstract class GUIOverlay : IGUIComponent
    {

        public override void Draw(GUIEvent guievent)
        {

        }
    }



}
