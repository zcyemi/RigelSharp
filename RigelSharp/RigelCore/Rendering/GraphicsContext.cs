using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace RigelCore.Rendering
{
    class GraphicsContext : IDisposable
    {
        private Device m_device;
        private SwapChain m_swapchain;
        private DeviceContext m_context;

        private Texture2D m_backBuffer;
        private Texture2D m_depthBuffer;

        private RenderTargetView m_renderTargetView;
        private DepthStencilView m_depthStencilView;

        

        public GraphicsContext()
        {

        }

        public void Render()
        {

        }

        public void Dispose()
        {

        }

        public void NeedResize()
        {

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
