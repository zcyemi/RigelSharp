using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Rendering
{
    public abstract class GraphicsContextBase
    {
        public event Action EventPreResizeBuffer = delegate { };
        public event Action EventPostResizeBuffer = delegate { };


        public abstract void Dispose();

        public abstract void CreateRenderTarget(IntPtr handle, int width, int height, bool windowed = true, bool allowModeSwitch = false);

        protected Dictionary<CommandBufferStage, List<ICommandBuffer>> m_commandBuffer = new Dictionary<CommandBufferStage, List<ICommandBuffer>>();

        protected GraphicsContextBase()
        {
            m_commandBuffer.Add(CommandBufferStage.Default, new List<ICommandBuffer>());
            m_commandBuffer.Add(CommandBufferStage.PostRender, new List<ICommandBuffer>());
            m_commandBuffer.Add(CommandBufferStage.PreRender, new List<ICommandBuffer>());
        }

        protected void onPreResizeBuffer()
        {
            EventPreResizeBuffer.Invoke();
        }

        protected void onPostResizeBuffer()
        {
            EventPostResizeBuffer.Invoke();
        }

        public abstract void Render(Action<IGraphicsImmediatelyContext> immediateDrall = null);

        public abstract IGraphicsImmediatelyContext ImmediatelyContext { get; }


#region CommandBuffer
        public void ClearAllCommandBuffer()
        {
            foreach (var bufferlist in m_commandBuffer.Values)
            {
                bufferlist.ForEach((x) => { x.Dispose(); });
                bufferlist.Clear();
            }

            m_commandBuffer.Clear();
        }

        public void RegisterCommandBuffer(CommandBufferStage stage, ICommandBuffer commandbuffer)
        {
            var buffers = m_commandBuffer[stage];
            if (!buffers.Contains(commandbuffer))
            {
                buffers.Add(commandbuffer);
            }
        }

        public void RemoveCommandBuffer(CommandBufferStage stage, ICommandBuffer commandBuffer)
        {
            var buffers = m_commandBuffer[stage];
            if (buffers.Contains(commandBuffer))
            {
                buffers.Remove(commandBuffer);
                commandBuffer.Dispose();
            }
        }

        public void ClearCommandBuffer(CommandBufferStage stage)
        {
            var buffers = m_commandBuffer[stage];
            foreach (var cmdbuffer in buffers)
            {
                cmdbuffer.Dispose();
            }
            buffers.Clear();
        }


        #endregion
    }
}
