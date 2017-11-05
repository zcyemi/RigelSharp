using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUITestComponent : IGUIComponent
    {
        public override void Draw(RigelEGUIEvent guievent)
        {
            GUILayout.BeginArea(new Vector4(50, 50, 400, 300));
            GUILayout.Button("Button1");
            if (GUILayout.Button("Button2"))
            {
                Distroy = true;
            }
            GUILayout.EndArea();
        }

    }
}
