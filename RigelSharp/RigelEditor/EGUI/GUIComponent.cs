using SharpDX;
using System;

namespace RigelEditor.EGUI
{
    public class GUIMenu
    {

    }


    public abstract class IGUIComponent
    {
        public int BufferRectStart;
        public int BufferRectEnd;
        public int BufferTextStart;
        public int BufferTextEnd;

        public int BufferRectCount { get { return BufferRectEnd - BufferRectStart; } }
        public int BufferTextCount { get { return BufferTextEnd - BufferTextStart; } }

        public bool InitDrawed = false;
        public bool Distroy { get; protected set; }

        public void Show()
        {
            GUI.DrawComponent(this);
        }

        public abstract void Draw(RigelEGUIEvent guievent);
    }

    public class GUIMessageBox:IGUIComponent
    {
        public string title = "MessageBox";
        public string info = "No Content";
        public string buttonConfirm = "Confirm";
        public string buttonCancel = "Cancel";

        private Action m_callbackConfirm;
        private Action m_callbackCancel;


        public GUIMessageBox(string title,string info,Action cbconfirm = null,Action cbcancel = null,string btnconfirm = null,string btncancel = null)
        {
            this.title = title ?? this.title;
            this.info = info ?? this.info;

            m_callbackConfirm = cbconfirm;
            m_callbackCancel = cbcancel;
            buttonConfirm = btnconfirm ?? buttonConfirm;
            buttonCancel = btncancel ?? buttonCancel;
        }

        public override void Draw(RigelEGUIEvent guievent)
        {
            //GUI.BeginGroup(new Vector4(200, 200, 500, 200), true);
            var rect = new Vector4(0, 0, 500, 200);
            //background
            GUI.DrawRect(rect);
            //header
            rect.W = 25;
            GUI.DrawRect(rect);
            GUI.Label(rect, title);

            //info
            rect.W = 125;
            rect.Y = 25;
            GUI.Label(rect, info);

            //buttons
            rect.Y += 125;
            rect.W = 25;
            rect.X = 300;
            rect.Z = 70;
            if (GUI.Button(rect, buttonConfirm))
            {
                if (m_callbackConfirm != null) m_callbackConfirm.Invoke();
                Distroy = true;
            }
            rect.X += 80;
            if (GUI.Button(rect, buttonCancel))
            {
                if (m_callbackCancel != null) m_callbackCancel.Invoke();
                Distroy = true;
            }

            //GUI.EndGroup();
        }
    }

    public class GUIDialog:IGUIComponent
    {
        public string title;
        public Action<RigelEGUIEvent> ongui;

        public override void Draw(RigelEGUIEvent guievent)
        {

        }
    }
}
