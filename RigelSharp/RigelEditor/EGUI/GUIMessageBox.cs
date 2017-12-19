using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RigelCore;

namespace RigelEditor.EGUI
{
    public sealed class GUIMessageBox : IGUIComponent
    {
        public string title = "MessageBox";
        public string info = "No Content";
        public string buttonConfirm = "Confirm";
        public string buttonCancel = "Cancel";

        private Action m_callbackConfirm;
        private Action m_callbackCancel;


        public GUIMessageBox(string title, string info, Action cbconfirm, Action cbcancel = null, string btnconfirm = null, string btncancel = null)
        {
            this.title = title ?? this.title;
            this.info = info ?? this.info;

            m_callbackConfirm = cbconfirm;
            m_callbackCancel = cbcancel;
            buttonConfirm = btnconfirm ?? buttonConfirm;
            buttonCancel = btncancel ?? buttonCancel;
        }

        public override void Draw(GUIEvent guievent)
        {
            var rect = new Vector4(0,0, 500, 150).CenterPos(GUI.Context.baseRect.Size());

            GUILayout.BeginArea(rect,GUIStyle.Current.DockContentColor,GUIOption.Border(null));
            {
                rect.W = GUILayout.s_svLineHeight;
                GUI.Rect(rect, GUIStyle.Current.ColorActiveD);
                GUILayout.Text(title);
                GUILayout.Text(info, (Vector4?)null, GUIOption.Width(480));

                var off = GUILayout.s_ctx.currentLayout.Offset;

                GUILayout.Space(120 - off.Y);

                bool hascancel = m_callbackCancel != null;
                GUILayout.BeginHorizontal();
                if (!hascancel)
                {
                    GUILayout.Indent(200);
                    if (GUILayout.Button(buttonConfirm, GUIOption.Width(100), GUIOption.Border()))
                    {
                        if (m_callbackConfirm != null) m_callbackConfirm.Invoke();
                        OnDestroy();
                    }
                }
                else
                {
                    GUILayout.Indent(150);
                    if (GUILayout.Button(buttonConfirm, GUIOption.Width(100), GUIOption.Border()))
                    {
                        if (m_callbackConfirm != null) m_callbackConfirm.Invoke();
                        OnDestroy();
                    }
                    if (GUILayout.Button(buttonCancel, GUIOption.Width(100), GUIOption.Border()))
                    {
                        if (m_callbackCancel != null) m_callbackCancel.Invoke();
                        OnDestroy();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        public override string ToString()
        {
            return "[MessageBox]" + title;
        }
    }
}
