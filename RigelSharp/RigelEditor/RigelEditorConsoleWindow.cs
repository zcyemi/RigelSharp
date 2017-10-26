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
            RigelEGUI.DrawChar(new Vector4(Position + Vector2.One *50, 100, 100),'R', new Vector4(1, 0, 0, 1));

            RigelEGUI.DrawText(new Vector4(Position.X + 3,Position.Y +3, 250, 20), "Hello World RigelSharp ! @Yemi", new Vector4(1, 0, 0, 1));
            RigelEGUI.DrawTextDebug(new Vector4(Position.X + 10, Position.Y + 30, 128,128), new Vector4(1, 0, 0, 1));
            //RigelEGUI.DrawRect(new Vector4(200, 10, 100, 100), new Vector4(0.4f, 0.1f, 0.7f, 1.0f));
        }
    }
}
