using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;
using SharpDX;

namespace RigelEditor
{
    public class RigelEditorConsoleWindow : RigelEditorGUIWindow
    {

        public override void OnGUI()
        {
            RigelEditorGUI.DrawRect(new Vector4(200, 10, 100, 100), new Vector4(0.4f, 0.1f, 0.7f, 1.0f));
        }
    }
}
