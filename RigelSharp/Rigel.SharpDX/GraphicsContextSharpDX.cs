using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Rendering;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Win32;
using SharpDX.Mathematics;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


namespace Rigel.SharpDX
{

    public class GraphicsContextSharpDX : GraphicsContextBase,IGraphicsImmediatelyContext
    {

        private Device m_device;
        private SwapChain m_swapchain;
        private SwapChainDescription m_swapchainDesc;
        private DeviceContext m_context;

        private Texture2D m_backBuffer;
        private RenderTargetView m_renderTargetView;
        private Texture2D m_depthBuffer;
        private DepthStencilView m_depthcStencilView;

        private ShaderResourceView m_srvBackBuffer;

        private Viewport m_viewport;


        private int m_resizeWidth;
        private int m_resizeHeight;
        private bool m_needResize = true;


        public override IGraphicsImmediatelyContext ImmediatelyContext
        {
            get
            {
                return this;
            }
        }

        #region ImmediatelyContext

        public void Clear(Vector4 color)
        {
            m_context.ClearRenderTargetView(m_renderTargetView, new RawColor4(color.x,color.y,color.z,color.w));
        }

        #endregion



        internal GraphicsContextSharpDX()
        {

        }




        public override void CreateRenderTarget(IntPtr handle, int width, int height, bool windowed = true, bool allowModeSwitch = false)
        {
            if(m_swapchain != null)
            {
                throw new Exception("main rendertarget iscreated");
            }

            m_swapchainDesc = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = windowed,
                OutputHandle = handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput | Usage.ShaderInput,
                Flags = allowModeSwitch ? SwapChainFlags.AllowModeSwitch : SwapChainFlags.None
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, m_swapchainDesc, out m_device, out m_swapchain);
            m_context = m_device.ImmediateContext;

            var factory = m_swapchain.GetParent<Factory>();
            factory.MakeWindowAssociation(handle, WindowAssociationFlags.IgnoreAll);
            factory.Dispose();

            //first resetbuffer
            m_resizeWidth = width;
            m_resizeHeight = height;
            DoResizeBuffer();
        }


        private void DoResizeBuffer()
        {
            //clear all command buffer
            onPreResizeBuffer();

            Utilities.Dispose(ref m_backBuffer);
            Utilities.Dispose(ref m_renderTargetView);
            Utilities.Dispose(ref m_depthBuffer);
            Utilities.Dispose(ref m_depthcStencilView);

            Utilities.Dispose(ref m_srvBackBuffer);
            //Console.WriteLine($"RSZ w:{m_resizeWidth} h:{m_resizeHeight}");

            m_swapchain.ResizeBuffers(m_swapchainDesc.BufferCount, m_resizeWidth, m_resizeHeight, Format.Unknown, SwapChainFlags.None);
            m_backBuffer = Texture2D.FromSwapChain<Texture2D>(m_swapchain, 0);

            //rtv
            m_renderTargetView = new RenderTargetView(m_device, m_backBuffer);

            //back buffer srv
            m_srvBackBuffer = new ShaderResourceView(m_device, m_backBuffer);

            //dsv
            m_depthBuffer = new Texture2D(m_device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = m_backBuffer.Description.Width,
                Height = m_backBuffer.Description.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            m_depthcStencilView = new DepthStencilView(m_device, m_depthBuffer);

            m_viewport = new Viewport(0, 0, m_resizeWidth, m_resizeHeight);
            m_context.Rasterizer.SetViewport(m_viewport);
            m_context.OutputMerger.SetTargets(m_depthcStencilView, m_renderTargetView);

            onPostResizeBuffer();
        }

        public override void Dispose()
        {


            if (m_depthcStencilView != null) m_depthcStencilView.Dispose();
            if (m_depthBuffer != null) m_depthBuffer.Dispose();
            if (m_renderTargetView != null) m_renderTargetView.Dispose();
            if (m_backBuffer != null) m_backBuffer.Dispose();

            if (m_swapchain != null) m_swapchain.Dispose();
            if (m_context != null) m_context.Dispose();
            if (m_device != null) m_device.Dispose();
        }

        public override void Render(Action<IGraphicsImmediatelyContext> immediateDrall = null)
        {
            if (m_needResize)
            {
                DoResizeBuffer();
                m_needResize = false;
            }

            //clear
            m_context.ClearDepthStencilView(m_depthcStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            m_context.ClearRenderTargetView(m_renderTargetView, Color.Black);


            //immediate draw
            if (immediateDrall != null)
            {
                immediateDrall.Invoke(ImmediatelyContext);
            }


            //do render
            //{
            //    m_commandBuffer[CommandBufferStage.PreRender].ForEach((x) => { x.Render(this); });
            //    m_commandBuffer[CommandBufferStage.Default].ForEach((x) => { x.Render(this); });
            //    m_commandBuffer[CommandBufferStage.PostRender].ForEach((x) => { x.Render(this); });
            //}

            m_swapchain.Present(1, PresentFlags.None);
        }
    }
}
