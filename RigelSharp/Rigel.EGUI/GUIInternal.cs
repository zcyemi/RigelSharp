using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


using Rigel;

namespace Rigel.EGUI
{
    public static class GUIInternal
    {
        private static GUICtx s_ctx;

        private static IGUIEventHandler s_eventHandler = null;
        private static IGUIGraphicsBind s_graphicsBind = null;

        private static List<GUIDrawStage> s_drawStages;

        public static IGUIGraphicsBind GraphicsBind { get { return s_graphicsBind; } }


        public static void Init(IRigelWindowEventHandler eventHandler,IGUIGraphicsBind bind,IFontInfo fontinfo)
        {
            Init(new GUIEventHandler(eventHandler), bind, fontinfo);
        }

        public static void Init(IGUIEventHandler eventHandler,IGUIGraphicsBind graphicsBind,IFontInfo fontinfo)
        {
            s_eventHandler = eventHandler;
            s_graphicsBind = graphicsBind;

            s_ctx = new GUICtx();
            s_ctx.Font = fontinfo;
            GUI.Context = s_ctx;

            s_drawStages = new List<GUIDrawStage>();
            s_drawStages.Add(new GUIDrawStageOverlay("Overlay", 1));
            s_drawStages.Add(new GUIDrawStageMain("Main", 499));
            s_drawStages.Sort((a, b) => { return a.Order.CompareTo(b.Order); });

            eventHandler.EventUpdate += Update;

        }


        public static void Release()
        {
            GUI.Context = null;

            s_drawStages.Clear();
        }

        public static void Update(GUIEvent guievent)
        {
            s_graphicsBind.UpdateGUIParams(guievent.RenderWidth, guievent.RenderHeight);


            //init frame
            GUI.Context.Frame(guievent, guievent.RenderWidth, guievent.RenderHeight);

            foreach(var stage in s_drawStages)
            {
                stage.Draw(guievent);
            }


            for(int i= s_drawStages.Count-1; i>=0; i--)
            {
                s_drawStages[i].SyncBuffer(s_graphicsBind);
            }

            GUI.Context.EndFrame();
            if (GUI.Context.TextureStorage.Changed)
            {
                GraphicsBind.SetDynamicBufferTexture(GUI.Context.TextureStorage.BufferData,GUI.Context.TextureStorage.BufferData.Count);
            }

        }

        [TODO("Refactoring","not impl")]
        public static void SetCursor(object cursor)
        {
            //if (s_eguictx == null) return;
            //s_eguictx.Form.Cursor = cursor;
        }
    }
}
