using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;

using SharpDX.Windows;

namespace RigelEditor.Platforms
{
    class GUIEventHandlerSharpDX : IGUIEventHandler
    {
        public event Action<GUIEvent> EventUpdate = delegate { };

        private RenderForm m_form;

        EventHandler<EventArgs> handlerUserResize;
        System.Windows.Forms.KeyEventHandler handlerKeyDown;
        System.Windows.Forms.KeyEventHandler handlerKeyUp;
        System.Windows.Forms.KeyPressEventHandler handlerKeyPress;
        System.Windows.Forms.MouseEventHandler handlerMouseMove;
        System.Windows.Forms.MouseEventHandler handlerMouseDown;
        System.Windows.Forms.MouseEventHandler handlerMouseUp;
        System.Windows.Forms.MouseEventHandler handlerMouseClick;
        System.Windows.Forms.MouseEventHandler handlerMouseDoubleClick;
        System.Windows.Forms.MouseEventHandler handlerMouseWheel;
        System.Windows.Forms.DragEventHandler handlerDragEnter;
        System.Windows.Forms.DragEventHandler handlerDragDrop;


        public GUIEventHandlerSharpDX()
        {
            handlerUserResize       = (sender, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.Resize, e));
            };
            handlerKeyDown          = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.KeyDown, e));
            };
            handlerKeyUp            = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.KeyUp, e));
            };
            handlerKeyPress         = (s, e) =>
            {
                //OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.KeyPress,e));
            };
            handlerMouseMove        = (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    EventUpdate(new GUIEvent(RigelEGUIEventType.MouseDragUpdate, e));
                }
                else
                {
                    EventUpdate(new GUIEvent(RigelEGUIEventType.MouseMove, e));
                }
            };
            handlerMouseDown        = (s, e) =>
            {
                var t = e;
                EventUpdate(new GUIEvent(RigelEGUIEventType.MouseDown, e));
            };
            handlerMouseUp          = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.MouseUp, e));
            };
            handlerMouseClick       = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.MouseClick, e));
            };
            handlerMouseDoubleClick = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.MouseDoubleClick, e));
            };
            handlerMouseWheel       = (s, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.MouseWheel, e));
            };
            handlerDragEnter        = (s, e) =>
            {
                EditorUtility.Log("event drag enter");
            };
            handlerDragDrop         = (s, e) =>
            {
                EditorUtility.Log("event drag drop");
            };
        }


        public void RegisterEvent(RenderForm form)
        {
            m_form = form;

            m_form.UserResized      += handlerUserResize;
            m_form.KeyDown          += handlerKeyDown;
            m_form.KeyUp            += handlerKeyUp;
            m_form.KeyPress         += handlerKeyPress;
            m_form.MouseMove        += handlerMouseMove;
            m_form.MouseDown        += handlerMouseDown;
            m_form.MouseUp          += handlerMouseUp;
            m_form.MouseClick       += handlerMouseClick;
            m_form.MouseDoubleClick += handlerMouseDoubleClick;
            m_form.MouseWheel       += handlerMouseWheel;
            m_form.DragEnter        += handlerDragEnter;
            m_form.DragDrop         += handlerDragDrop;
        }

        public void UnRegister()
        {
            if (m_form == null) return;
            m_form.UserResized      -= handlerUserResize;
            m_form.KeyDown          -= handlerKeyDown;
            m_form.KeyUp            -= handlerKeyUp;
            m_form.KeyPress         -= handlerKeyPress;
            m_form.MouseMove        -= handlerMouseMove;
            m_form.MouseDown        -= handlerMouseDown;
            m_form.MouseUp          -= handlerMouseUp;
            m_form.MouseClick       -= handlerMouseClick;
            m_form.MouseDoubleClick -= handlerMouseDoubleClick;
            m_form.MouseWheel       -= handlerMouseWheel;
            m_form.DragEnter        -= handlerDragEnter;
            m_form.DragDrop         -= handlerDragDrop;
        }
    }
}
