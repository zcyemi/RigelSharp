using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public enum GUIDragStateStage
    {
        Enter,
        Exit,
        Update,
        None,
    }

    public class GUIDragState
    {
        private Vector2 m_offset;
        private bool m_ondrag = false;
        public Vector2 OffSet { get { return m_offset; } }
        public GUIDragStateStage Stage { get; private set; } = GUIDragStateStage.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">absolute rect</param>
        /// <returns></returns>
        public bool OnDrag(Vector4 rect)
        {
            if (GUI.Event.Used) return false;
            bool constains = GUIUtility.RectContainsCheck(rect, GUI.Event.Pointer);
            return OnDrag(constains);
        }

        public bool OnDrag(bool constainsCheck = true)
        {
            if (GUI.Event.Used) return false;

            var e = GUI.Event;
            if (e.EventType == RigelEGUIEventType.MouseDragEnter)
            {
                if (constainsCheck)
                {
                    m_ondrag = true;
                    e.Use();
                    m_offset = Vector2.Zero;
                    Stage = GUIDragStateStage.Enter;
                    return true;
                }
            }
            else if (e.EventType == RigelEGUIEventType.MouseDragLeave)
            {
                if (m_ondrag)
                {
                    e.Use();
                    m_ondrag = false;
                    m_offset = e.DragOffset;
                    Stage = GUIDragStateStage.Exit;
                    return true;
                }
            }
            else if (e.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if (m_ondrag)
                {
                    e.Use();
                    m_offset = e.DragOffset;
                    Stage = GUIDragStateStage.Update;
                    return true;
                }

            }
            else
            {
                Stage = GUIDragStateStage.None;
            }

            return false;
        }
    }
}
