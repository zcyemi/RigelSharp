using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.EGUI;

namespace RigelEngine.Module
{
    public class ModuleEngineGUI : IRigelEngineModule
    {

        private FontInfo m_font = new FontInfo("arial.ttf");

        public void Init()
        {
            GUIInternal.Init(RigelEngineApp.App.MainWindow.EvnetHandler,new EngineGUIGraphicsBind(), m_font);
        }

        public void Update()
        {

        }

        public void Destroy()
        {
            if(m_font != null)
            {
                m_font.Dispose();
            }
            GUIInternal.Release();
        }


        private class EngineGUIGraphicsBind : IGUIGraphicsBind
        {
            public bool NeedRebuildCommandList
            {
                get { return false;}
                set { }
            }

            public IGUIBuffer CreateBuffer()
            {
                return new GUIBufferVertice();
            }

            public void SetDynamicBufferTexture(object vertexdata, int length)
            {
            }

            public void SyncDrawTarget(GUIDrawStage stage, GUIDrawTarget drawtarget)
            {
            }

            public void UpdateGUIParams(int width, int height)
            {
            }
        }
    }

    
}
