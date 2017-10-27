using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;
using SharpDX;

namespace RigelEditor
{
    public class RigelEditorConsoleWindow : RigelEGUIWindow
    {
        public override void OnStart()
        {
            m_debugColor = new Vector4(0, 1, 1, 1);
        }

        public override void OnGUI()
        {
            //RigelEGUI.DrawText(new Vector4(Position.X,Position.Y, 250, 20),WindowTitle,Vector4.One);
            //RigelEGUI.DrawRect(new Vector4(200, 10, 100, 100), new Vector4(0.4f, 0.1f, 0.7f, 1.0f));
        }
    }
}
