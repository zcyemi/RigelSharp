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

namespace RigelEditor
{
    public class RigelGraphics : IDisposable
    {
        private RenderForm m_form;

        private Device m_device;
        public Device Device { get { return m_device; } }
        private SwapChain m_swapchain;
        private SwapChainDescription m_swapChainDescription;
        private DeviceContext m_context;
        public DeviceContext ImmediateContext { get { return m_context; } }


        private Texture2D m_backBuffer;
        private RenderTargetView m_renderTargetView;
        private Texture2D m_deptBuffer;
        private DepthStencilView m_depthStencilView;

        private bool m_needResize = true;


        public void CreateWithSwapChain(RenderForm form)
        {
            m_form = form;

            m_swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width,form.ClientSize.Height,new Rational(60,1),Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, m_swapChainDescription, out m_device, out m_swapchain);
            m_context = m_device.ImmediateContext;

            //imnore windows events
            var factory = m_swapchain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);
            factory.Dispose();

            m_form.UserResized += (sender, e) => { OnWindowResize(); };
        }

        public void Resize()
        {
            Utilities.Dispose(ref m_backBuffer);
            Utilities.Dispose(ref m_renderTargetView);
            Utilities.Dispose(ref m_deptBuffer);
            Utilities.Dispose(ref m_depthStencilView);

            m_swapchain.ResizeBuffers(m_swapChainDescription.BufferCount, m_form.ClientSize.Width, m_form.ClientSize.Height, Format.Unknown, SwapChainFlags.None);
            m_backBuffer = Texture2D.FromSwapChain<Texture2D>(m_swapchain, 0);

            m_renderTargetView = new RenderTargetView(m_device, m_backBuffer);
            m_deptBuffer = new Texture2D(m_device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = m_form.ClientSize.Width,
                Height = m_form.ClientSize.Height,
                SampleDescription = new SampleDescription(1,0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags =  CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            m_depthStencilView = new DepthStencilView(m_device, m_deptBuffer);

            m_context.Rasterizer.SetViewport(new Viewport(0, 0, m_form.ClientSize.Width, m_form.ClientSize.Height,0.0f,1.0f));
            m_context.OutputMerger.SetTargets(m_depthStencilView, m_renderTargetView);

        }

        public void Render(Action drawfunc = null)
        {
            if (m_needResize)
            {
                Resize();
                m_needResize = false;
            }

            m_context.ClearDepthStencilView(m_depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            m_context.ClearRenderTargetView(m_renderTargetView, Color.Black);
            

            if (drawfunc != null) drawfunc.Invoke();

            m_swapchain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            m_depthStencilView.Dispose();
            m_deptBuffer.Dispose();
            m_renderTargetView.Dispose();
            m_backBuffer.Dispose();

            m_swapchain.Dispose();
            m_context.Dispose();
            m_device.Dispose();

        }

        public void OnWindowResize()
        {
            m_needResize = true;
        }


        public VertexShader CreateVertexShader(CompilationResult shadercode)
        {
            return new VertexShader(m_device, shadercode);
        }

        public PixelShader CraetePixelShader(CompilationResult shadercode)
        {
            return new PixelShader(m_device, shadercode);
        }

        public InputLayout CreateInputLayout(ShaderSignature signature,InputElement[] elements)
        {
            return new InputLayout(m_device, signature, elements);
        }

        public void ExecRenderCall(RigelDrawCall drawcall)
        {
            if(drawcall.ShaderVertex != null)
            {
                m_context.VertexShader.Set(drawcall.ShaderVertex);
            }
            if(drawcall.ShaderPixel != null)
            {
                m_context.PixelShader.Set(drawcall.ShaderPixel);
            }
            if(drawcall.Layout != null)
            {
                m_context.InputAssembler.InputLayout = drawcall.Layout;
            }
            if(drawcall.InputPrimitiveTopology != null)
            {
                m_context.InputAssembler.PrimitiveTopology = (PrimitiveTopology)drawcall.InputPrimitiveTopology;
            }
            if(drawcall.VertexBufferBinding != null)
            {
                m_context.InputAssembler.SetVertexBuffers(0,(VertexBufferBinding)drawcall.VertexBufferBinding);
            }

        }

    }
}
