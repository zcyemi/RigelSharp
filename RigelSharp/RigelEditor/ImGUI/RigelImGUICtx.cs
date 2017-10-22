using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

using RigelEditor;

namespace RigelEditor.ImGUI
{
    public class RigelImGUICtx:IDisposable
    {
        private RenderForm m_form;
        private RigelImGUIGraphicsBind m_graphicsBind = null;
        private RigelFont m_font = null;

        public RigelImGUICtx(RenderForm form,RigelGraphics graphics)
        {
            m_form = form;
            
            m_graphicsBind = new RigelImGUIGraphicsBind(graphics);

            m_font = new RigelFont("arial.ttf");
            m_graphicsBind.CrateFontTexture(m_font);

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            m_form.UserResized += (sender, e) => {
                m_graphicsBind.UpdateGUIParams(m_form.ClientSize.Width, m_form.ClientSize.Height);
            };
        }



        public void Render(RigelGraphics graphics)
        {
            m_graphicsBind.Render(graphics);
        }

        public void Dispose()
        {
            m_graphicsBind.Dispose();

            m_form = null;
        }
    }

    public static class RigelImGUI
    {
        public static RigelImGUICtx s_currentCtx = null;

        public static void NewFrame(RigelImGUICtx ctx)
        {
            s_currentCtx = ctx;
        }

        public static void EndFrame()
        {
            s_currentCtx = null;
        }
    }

}
