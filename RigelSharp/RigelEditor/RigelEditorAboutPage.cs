using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;
using SharpDX;

namespace RigelEditor
{
    class RigelEditorAboutPage : RigelEGUIWindow
    {
        [RigelEGUIMenuItem("Aboud")]
        public static void ShowAboudPage()
        {

        }

        public override void OnStart()
        {
            Position.X = 400;
            Position.Y = 50;
        }

        public override void OnGUI()
        {
            RigelEGUI.DrawRect(new Vector4(200, 100, 200, 50), Vector4.One);
        }

        
    }
}
