using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RigelEditor.EGUI;
using RigelEditor;
using RigelCore;
using RigelCore.Rendering;

namespace RigelEditor.Player
{
    public class PlayerEditorWindow : GUIDockContentBase
    {
        private static PlayerEditorWindow s_playerWindow = null;

        [EditorMenuItem("Window","Game")]
        public static void ShowPlayerWindow()
        {
            if(s_playerWindow == null)
            {
                s_playerWindow = new PlayerEditorWindow();

                var dockmgr = RigelEditorApp.Instance.EditorGUI.DockManager;
                if(dockmgr.FindDockContent<PlayerEditorWindow>() == null)
                {
                    dockmgr.AddNewContent(s_playerWindow);
                }
            }
        }

        public PlayerEditorWindow()
        {
            Title = "Player";
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Button("Start");
            GUILayout.Button("Pause");
            GUILayout.Button("Stop");

            GUILayout.EndHorizontal();

            var sr = GUILayout.SizeRemain;
            var rect = new Vector4(GUILayout.CurrentLayout.Offset,sr.X,sr.Y);
            var rectab = GUILayout.GetRectAbsolute(rect);

            GUI.RectA(rectab, RigelColor.Black);
            GUILayout.DrawTexture(rectab, RenderTextureIdentifier.DefaultDepthStencilView);

        }
    }
}
