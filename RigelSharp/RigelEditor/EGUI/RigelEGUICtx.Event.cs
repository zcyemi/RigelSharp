using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public class RigelEGUIEvent
    {
        public bool Used { get; set; }
        public RigelEGUIEventType EventType { get; private set; }
        public RigelEGUIEvent(RigelEGUIEventType eventtype)
        {
            EventType = eventtype;
        }

        public void Use()
        {
            Used = true;
        }
    }

    public enum RigelEGUIEventType
    {
        MouseClick,
        MouseWheel,
        MouseDoubleClick,
        MouseUp,
        MouseDown,
        KeyPress,
        KeyDown,
        KeyUp,
    }

    public partial class RigelEGUICtx
    {
        private void RegisterEvent()
        {
            m_form.UserResized += (sender, e) => {
                m_graphicsBind.UpdateGUIParams(m_form.ClientSize.Width, m_form.ClientSize.Height);
                RigelUtility.Log("event resize");
            };
            m_form.KeyDown += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.KeyDown));
            };
            m_form.KeyUp += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.KeyUp));
            };
            m_form.KeyPress += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.KeyPress));
            };
            m_form.MouseDown += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.MouseDown));
            };
            m_form.MouseUp += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.MouseUp));
            };
            m_form.MouseClick += (s, e) => {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.MouseClick));
            };
            m_form.MouseDoubleClick += (s, e) => {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.MouseDoubleClick));
            };
            m_form.MouseWheel += (s, e) =>
            {
                OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.MouseWheel));
            };
            m_form.DragEnter += (s, e) =>
            {
                RigelUtility.Log("event drag enter");
            };
            m_form.DragDrop += (s, e) =>
            {
                RigelUtility.Log("event drag drop");
            };
        }
    }
}
