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
            RigelEGUI.DrawRect(new Vector4(0, 0, 20, 20), RigelColor.RGBA(20, 42, 87, 128));

            if(RigelEGUI.Button(new Vector2(100,30),new Vector2(100, 20), "Button1", Vector4.Zero, Vector4.One))
            {
                Console.WriteLine("Button1 click");
            }

            //RigelEGUI.DrawText(new Vector4(Position.X,Position.Y, 250, 20),WindowTitle,Vector4.One);
            //RigelEGUI.DrawRect(new Vector4(200, 10, 100, 100), new Vector4(0.4f, 0.1f, 0.7f, 1.0f));
        }
    }
}
