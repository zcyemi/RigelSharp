using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

using Rigel;
using Rigel.Rendering;

namespace RigelEditor
{
    public class EditorGraphicsManager : Singleton<EditorGraphicsManager>,IDisposable
    {
        public event Action EventReleaseCommandList = delegate { };


        private GraphicsContext m_graphics;

        public GraphicsContext Graphics { get { return m_graphics; } }

        public EditorGraphicsManager()
        {

        }

        public void Init()
        {
            m_graphics = new GraphicsContext();

            var form = RigelEditorApp.Instance.Form;
            m_graphics.CreateWithSwapChain(form.Handle, form.ClientSize.Width, form.ClientSize.Height);

            form.UserResized += FormResize;
        }

        private void FormResize(object sender, EventArgs e)
        {
            var form = RigelEditorApp.Instance.Form;
            m_graphics.Resize(form.ClientSize.Width, form.ClientSize.Height);
        }

        public void Render(Action drawfunc = null)
        {

            m_graphics.Render();

            //if (m_needResize)
            //{
            //    Resize();
            //    m_needResize = false;

            //    NeedRebuildCommandList = true;
            //}

            //m_context.ClearDepthStencilView(m_depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            //m_context.ClearRenderTargetView(m_renderTargetView, Color.Black);
            

            //if (drawfunc != null) drawfunc.Invoke();

            //m_swapchain.Present(1, PresentFlags.None);

            //NeedRebuildCommandList = false;
        }

        public void Dispose()
        {
            m_graphics.Dispose();
        }


        //public VertexShader CreateVertexShader(CompilationResult shadercode)
        //{
        //    return new VertexShader(m_device, shadercode);
        //}

        //public PixelShader CraetePixelShader(CompilationResult shadercode)
        //{
        //    return new PixelShader(m_device, shadercode);
        //}

        //public InputLayout CreateInputLayout(ShaderSignature signature,InputElement[] elements)
        //{
        //    return new InputLayout(m_device, signature, elements);
        //}

        //public void ExecRenderCall(RigelDrawCall drawcall)
        //{
        //    if(drawcall.ShaderVertex != null)
        //    {
        //        m_context.VertexShader.Set(drawcall.ShaderVertex);
        //    }
        //    if(drawcall.ShaderPixel != null)
        //    {
        //        m_context.PixelShader.Set(drawcall.ShaderPixel);
        //    }
        //    if(drawcall.Layout != null)
        //    {
        //        m_context.InputAssembler.InputLayout = drawcall.Layout;
        //    }
        //    if(drawcall.InputPrimitiveTopology != null)
        //    {
        //        m_context.InputAssembler.PrimitiveTopology = (PrimitiveTopology)drawcall.InputPrimitiveTopology;
        //    }
        //    if(drawcall.VertexBufferBinding != null)
        //    {
        //        //m_context.InputAssembler.SetVertexBuffers(0,(VertexBufferBinding)drawcall.VertexBufferBinding);
        //    }

        //}

    }
}
