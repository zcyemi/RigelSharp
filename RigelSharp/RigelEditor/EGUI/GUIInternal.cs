using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using SharpDX;

namespace RigelEditor.EGUI
{
    internal static class GUIInternal
    {
        private static GUICtx s_ctx;

        private static EditorGUICtx s_eguictx;

        private static IGUIEventHandler s_eventHandler = null;

        private static List<GUIDrawStage> s_drawStages;

        public static void Init(IGUIEventHandler eventHandler)
        {
            s_eventHandler = eventHandler;
        }

        public static void Init(EditorGUICtx eguictx)
        {
            s_eguictx = eguictx;

            s_ctx = new GUICtx();
            s_ctx.Font = eguictx.Font;
            GUI.Context = s_ctx;

            s_drawStages = new List<GUIDrawStage>();
            s_drawStages.Add(new GUIDrawStageOverlay("Overlay", 1));
            s_drawStages.Add(new GUIDrawStageMain("Main", 499));

            s_drawStages.Sort((a, b) => { return a.Order.CompareTo(b.Order); });
        }

        public static void Release()
        {
            GUI.Context = null;

            s_drawStages.Clear();

        }

        public static void Update(GUIEvent guievent)
        {
            //init frame
            GUI.Context.Frame(guievent, s_eguictx.ClientWidth,s_eguictx.ClientHeight);

            foreach(var stage in s_drawStages)
            {
                stage.Draw(guievent);
            }


            for(int i= s_drawStages.Count-1; i>=0; i--)
            {
                s_drawStages[i].SyncBuffer(s_eguictx);
            }

            GUI.Context.EndFrame();
            if (GUI.Context.TextureStorage.Changed)
            {
                s_eguictx.GraphicsBind.SetDynamicBufferTexture(GUI.Context.TextureStorage.BufferData.ToArray(),GUI.Context.TextureStorage.BufferData.Count);
            }

        }

        public static void SetCursor(System.Windows.Forms.Cursor cursor)
        {
            if (s_eguictx == null) return;
            s_eguictx.Form.Cursor = cursor;
        }
    }
}
