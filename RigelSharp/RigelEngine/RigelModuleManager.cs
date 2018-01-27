using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEngine.Module;

namespace RigelEngine
{
    public class RigelModuleManager
    {


        private ModuleEditorGraphics m_moduleEditorGraphics;
        private ModuleEngineGUI m_moduleEngineGUI;

        public RigelModuleManager()
        {

        }

        public void Init(RigelEngineApp app)
        {
            m_moduleEditorGraphics = new ModuleEditorGraphics();
            m_moduleEditorGraphics.Init();

            m_moduleEngineGUI = new ModuleEngineGUI();
            m_moduleEngineGUI.Init();
        }

        public void Update()
        {
            m_moduleEngineGUI.Update();

            m_moduleEditorGraphics.Update();
        }

        public void Destroy()
        {
            m_moduleEngineGUI.Destroy();

            m_moduleEditorGraphics.Destroy();
        }
    }
}
