using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Context;

namespace RigelEngine
{
    public class RigelEngineApp
    {

        private static RigelEngineApp s_app = new RigelEngineApp();
        public static RigelEngineApp App
        {
            get { return s_app; }
        }

        private bool m_initSuccess = false;

        private IRigelGfxContext m_contextGfx = null;
        private IRigelWindowContext m_contextWindow = null;
        private IRigelWindow m_mainWindow = null;

        

        private RigelEngineApp()
        {
            Init();
        }

        private void Init()
        {
            m_contextWindow = RigelContextManager.GetWindowContext(PlatformEnum.Windows);
            m_contextGfx = RigelContextManager.GetGfxContext(GraphicsAPIEnum.DirectX, PlatformEnum.Windows);

            if(m_contextWindow != null && m_contextGfx != null)
            {
                m_initSuccess = true;
            }
        }

        public void Run()
        {
            if (!m_initSuccess) return;

            m_mainWindow = m_contextWindow.InitMainWindow("Rigel - 0.0.1");
            if (m_mainWindow == null) return;
            m_mainWindow.Run(() =>
            {

            });

            m_mainWindow.DestroyWindow();
        }

    }
}
