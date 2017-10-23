using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;
using SharpDX;

namespace RigelEditor
{
    class RigelEditorAboutPage : RigelEditorGUIWindow
    {
        [RigelEGUIMenuItem("Aboud")]
        public static void ShowAboudPage()
        {

        }

        public override void OnGUI()
        {
            RigelEditorGUI.DrawRect(new Vector4(200, 100, 200, 50), Vector4.One);

            RigelEditorGUI.DrawRect(new Vector4(220, 120, 40, 40), new Vector4(1,0,1,1));
        }

        
    }
}
