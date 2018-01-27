using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Rigel;
using Rigel.Rendering;

namespace RigelEngine.Module
{
    public class ModuleEditorGraphics: IRigelEngineModule
    {
        private GraphicsContextBase m_graphicsContext = null;

        public void Init()
        {
            m_graphicsContext = RigelEngineApp.App.ContextGFX.CreateGraphicsContext(null);

            Console.WriteLine(m_graphicsContext);
            m_graphicsContext.CreateRenderTarget(RigelEngineApp.App.MainWindow.GetWindowHandler(), RigelEngineApp.App.WindowWidth, RigelEngineApp.App.WindowHeight) ;

        }


        public void Update()
        {
            m_graphicsContext.Render();
        }

        public void Destroy()
        {
            m_graphicsContext.Dispose();
        }
    }
}
