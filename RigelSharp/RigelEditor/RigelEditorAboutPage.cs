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

            m_debugColor = new Vector4(0, 0.2f, 1.0f, 1.0f);
        }

        public override void OnGUI()
        {
            
        }

        
    }
}
