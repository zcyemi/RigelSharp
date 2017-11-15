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
        public override void Draw(GUIEvent guievent)
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


    public class GUITestLayout
    {
        public static void Sample_GroupMixedWithArea()
        {
            var narea = GUI.Context.currentArea;
            narea.Y = 20;
            narea.W -= 20;

            var ngroup = GUI.Context.currentGroup.Rect;
            ngroup.Y = 20;
            ngroup.W -= 20;
            GUI.BeginGroup(ngroup, RigelColor.Random());
            GUILayout.BeginArea(narea);

            GUILayout.Button("BtnArea");

            GUI.Button(new Vector4(100, 0, 100, 20), "BtnGroup");

            GUI.BeginGroup(new Vector4(100, 100, 100, 100), RigelColor.Random());

            GUILayout.Button("BtnAra2");
            GUI.Button(new Vector4(0, 0, 100, 20), "BtnGroup2");
            GUI.EndGroup();

            GUILayout.BeginArea(new Vector4(200, 100, 100, 200), RigelColor.Random());
            GUILayout.BeginToolBar(20);
            GUILayout.Text("Thisisatoolbar");
            GUILayout.EndToolBar();

            GUILayout.EndArea();


            GUILayout.EndArea();
            GUI.EndGroup();
        }
    }
}
