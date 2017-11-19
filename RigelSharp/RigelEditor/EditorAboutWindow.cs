using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelEditor.EGUI;

namespace RigelEditor
{
    class EditorAboutWindow
    {
        [EditorMenuItem("Help","About")]
        public static void RegisterMenuItem()
        {
            GUI.DrawComponent(new GUIAboutDialog());
        }

        private class GUIAboutDialog : GUIDialog
        {
            public GUIAboutDialog()
            {
                m_title = "About RIGEL";
            }

            protected override void OnDraw()
            {


                if (!GUI.Event.Used)
                {
                    if (GUI.Event.IsMouseActiveEvent())
                    {
                        GUI.Event.Use();
                        OnDistroy();
                    }
                }
            }
        }

    }
}
