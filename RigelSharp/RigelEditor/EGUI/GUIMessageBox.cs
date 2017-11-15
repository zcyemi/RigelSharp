using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUIMessageBox : IGUIComponent
    {
        public string title = "MessageBox";
        public string info = "No Content";
        public string buttonConfirm = "Confirm";
        public string buttonCancel = "Cancel";

        private Action m_callbackConfirm;
        private Action m_callbackCancel;


        public GUIMessageBox(string title, string info, Action cbconfirm = null, Action cbcancel = null, string btnconfirm = null, string btncancel = null)
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
            GUI.BeginGroup(new Vector4(200, 200, 500, 200), null, true);
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

                Console.WriteLine(title + " confirm");
                Distroy = true;
            }
            rect.X += 80;
            if (GUI.Button(rect, buttonCancel))
            {
                if (m_callbackCancel != null) m_callbackCancel.Invoke();
                Console.WriteLine(title + " cancel");
                Distroy = true;
            }
            GUI.EndGroup();
        }

        public override string ToString()
        {
            return "[MessageBox]" + title;
        }
    }
}
