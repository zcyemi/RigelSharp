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
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.Resize, e));
            };
            handlerKeyDown          = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.KeyDown, e));
            };
            handlerKeyUp            = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.KeyUp, e));
            };
            handlerKeyPress         = (s, e) =>
            {
                //OnWindowEvent(new RigelEGUIEvent(RigelEGUIEventType.KeyPress,e));
            };
            handlerMouseMove        = (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseDragUpdate, e));
                }
                else
                {
                    PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseMove, e));
                }
            };
            handlerMouseDown        = (s, e) =>
            {
                var t = e;
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseDown, e));
            };
            handlerMouseUp          = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseUp, e));
            };
            handlerMouseClick       = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseClick, e));
            };
            handlerMouseDoubleClick = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseDoubleClick, e));
            };
            handlerMouseWheel       = (s, e) =>
            {
                PostProcessEvent(new GUIEvent(RigelEGUIEventType.MouseWheel, e));
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


        private bool m_lastFrameDrag = false;
        private RigelCore.Vector2 m_LastPointerDrag;

        private void PostProcessEvent(GUIEvent guievent)
        {
            //process drag
            if (guievent.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if (m_lastFrameDrag == false)
                {
                    guievent.EventType = RigelEGUIEventType.MouseDragEnter;
                    m_lastFrameDrag = true;
                }
            }
            else if (m_lastFrameDrag == true && (guievent.EventType & RigelEGUIEventType.MouseEvent) > 0)
            {
                m_lastFrameDrag = false;
                guievent.EventType = RigelEGUIEventType.MouseDragLeave;
            }

            if (guievent.IsMouseDragEvent())
            {
                if (guievent.EventType == RigelEGUIEventType.MouseDragUpdate)
                {
                    guievent.DragOffset = guievent.Pointer - m_LastPointerDrag;
                }
                m_LastPointerDrag = guievent.Pointer;
            }

            guievent.RenderWidth = m_form.ClientSize.Width;
            guievent.RenderHeight = m_form.ClientSize.Height;

            //dispatcher event
            EventUpdate(guievent);
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
