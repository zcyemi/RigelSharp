using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Win32;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace RigelCore.Rendering
{
    public class GraphicsContext : IDisposable
    {

        private int m_resizeWidth, m_resizeHeight;
        private bool m_needResize = true;

        private Device m_device;
        private SwapChain m_swapchain;
        private SwapChainDescription m_swapchainDesc;

        private DeviceContext m_context;

        private Texture2D m_backBuffer;
        private RenderTargetView m_renderTargetView;
        private Texture2D m_depthBuffer;
        private DepthStencilView m_depthcStencilView;

        private Viewport m_viewport;

        public Device Device { get { return m_device; } }
        

        public GraphicsContext()
        {

        }

        public void CreateWithSwapChain(IntPtr handle,int width,int height)
        {
            m_swapchainDesc = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, m_swapchainDesc, out m_device, out m_swapchain);
            m_context = m_device.ImmediateContext;

            var factory = m_swapchain.GetParent<Factory>();
            factory.MakeWindowAssociation(handle, WindowAssociationFlags.IgnoreAll);
            factory.Dispose();
        }

        public void Render()
        {
            //process resize
            if (m_needResize)
            {
                DoResizeBuffer();
                m_needResize = false;
            }


            m_context.ClearDepthStencilView(m_depthcStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            m_context.ClearRenderTargetView(m_renderTargetView, Color.DarkRed);

            m_swapchain.Present(1, PresentFlags.None);

        }

        private void DoResizeBuffer()
        {
            //clear all command buffer

            Utilities.Dispose(ref m_backBuffer);
            Utilities.Dispose(ref m_renderTargetView);
            Utilities.Dispose(ref m_depthBuffer);
            Utilities.Dispose(ref m_depthcStencilView);

            m_swapchain.ResizeBuffers(m_swapchainDesc.BufferCount, m_resizeWidth, m_resizeHeight, Format.Unknown, SwapChainFlags.None);
            m_backBuffer = Texture2D.FromSwapChain<Texture2D>(m_swapchain, 0);

            m_renderTargetView = new RenderTargetView(m_device, m_backBuffer);
            m_depthBuffer = new Texture2D(m_device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = m_backBuffer.Description.Width,
                Height = m_backBuffer.Description.Height,
                SampleDescription = new SampleDescription(1,0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            m_depthcStencilView = new DepthStencilView(m_device, m_depthBuffer);

            m_viewport = new Viewport(0, 0, m_resizeWidth, m_resizeHeight);
            m_context.Rasterizer.SetViewport(m_viewport);
            m_context.OutputMerger.SetTargets(m_depthcStencilView, m_renderTargetView);

        }
        
        public void Dispose()
        {
            if (m_depthcStencilView != null) m_depthcStencilView.Dispose();
            if (m_depthBuffer != null) m_depthBuffer.Dispose();
            if (m_renderTargetView != null) m_renderTargetView.Dispose();
            if (m_backBuffer != null) m_backBuffer.Dispose();

            if (m_swapchain != null) m_swapchain.Dispose();
            if (m_context != null) m_context.Dispose();
            if (m_device != null) m_device.Dispose();
        }

        public void Resize(int width,int height)
        {
            m_resizeWidth = width;
            m_resizeHeight = height;
            m_needResize = true;
        }


#region CommandBuffer
        public void RegisterCommandBuffer(CommandBufferStage stage,CommandBuffer commandbuffer)
        {

        }

        public void RemoveCommandBuffer(CommandBufferStage stage,CommandBuffer commandBuffer)
        {

        }

        public void ClearCommandBuffer(CommandBufferStage stage)
        {

        }

        public void ClearAllCommandBuffer()
        {

        }

#endregion
    }
}
