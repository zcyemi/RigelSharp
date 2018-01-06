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

        private static RigelEngineApp s_app = null;
        public static RigelEngineApp App
        {
            get {
                if(s_app == null)
                {
                    s_app = new RigelEngineApp();
                }
                return s_app;
            }
        }

        private bool m_initSuccess = false;

        private IRigelGfxContext m_contextGfx = null;
        private IRigelWindowContext m_contextWindow = null;
        private IRigelWindow m_mainWindow = null;

        private RigelModuleManager m_moduleManager = null;

        public IRigelGfxContext ContextGFX
        {
            get { return m_contextGfx; }
        }

        public IRigelWindow MainWindow
        {
            get { return m_mainWindow; }
        }

        public int WindowHeight
        {
            get { return m_mainWindow.GetWindowHeight(); }
        }
        public int WindowWidth
        {
            get { return m_mainWindow.GetWindowWidth(); }
        }



        private RigelEngineApp()
        {
            
        }

        public void Init()
        {
            m_contextWindow = RigelContextManager.GetWindowContext(PlatformEnum.Windows);
            m_mainWindow = m_contextWindow.InitMainWindow("Rigel - 0.0.1");
            if (m_mainWindow == null) return;

            m_contextGfx = RigelContextManager.GetGfxContext(GraphicsAPIEnum.DirectX, PlatformEnum.Windows);

            if(m_contextWindow != null && m_contextGfx != null)
            {
                m_initSuccess = true;

                m_moduleManager = new RigelModuleManager();
                m_moduleManager.Init(this);
            }
        }

        public void Run()
        {
            if (!m_initSuccess) return;

            
            m_mainWindow.Run(UpdateFunction);
            
        }

        public void UpdateFunction()
        {
            m_moduleManager.Update();
        }

        public void Destroy()
        {
            m_moduleManager.Destroy();

            m_mainWindow.DestroyWindow();
        }

    }
}
