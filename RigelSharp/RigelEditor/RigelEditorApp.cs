using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

using RigelEditor.ImGUI;

namespace RigelEditor
{
    public class RigelEditorApp
    {

        private RenderForm m_windowForm;
        private RigelGraphics m_graphics;
        private RigelImGUICtx m_imgui;


        public RigelEditorApp()
        {
            m_windowForm = new RenderForm("Rigel");
            m_windowForm.AllowUserResizing = true;

            m_graphics = new RigelGraphics();
            m_graphics.CreateWithSwapChain(m_windowForm);

            m_imgui = new RigelImGUICtx(m_windowForm,m_graphics);

        }

        public void EnterRunloop() {

            RenderLoop.Run(m_windowForm, () =>
            {

                m_graphics.Render(()=> {

                    m_imgui.Render(m_graphics);

                });
            });

            m_imgui.Dispose();


            m_graphics.Dispose();
            m_windowForm.Dispose();
        }

    }
}
