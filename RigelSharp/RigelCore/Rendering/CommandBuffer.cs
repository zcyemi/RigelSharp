using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;
using RigelCore.Engine;

using SharpDX.Direct3D11;


namespace RigelCore.Rendering
{
    public enum CommandBufferStage
    {
        Default,
        PreRender,
        PostRender,
    }

    public interface ICommandBuffer
    {
        void Dispose();
        void Render(GraphicsContext graphics);
    }

    public class RawCommandBuffer: ICommandBuffer
    {
        private CommandList m_commandList;
        public RawCommandBuffer(CommandList commandList)
        {
            m_commandList = commandList;
        }

        public void ReplaceCommandList(CommandList cmdlist)
        {
            m_commandList = cmdlist;
        }


        public void Render(GraphicsContext graphics)
        {
            if (m_commandList != null)
                graphics.ImmediateContext.ExecuteCommandList(m_commandList, false);
        }


        //do not dispose here
        public void Dispose()
        {
            m_commandList = null;
        }
    }

    public class CommandBuffer:ICommandBuffer
    {

        public CommandBuffer()
        {

        }


        public void ClearRenderTarget(Vector4 color)
        {

        }

        public void Dispose()
        {

        }

        public void Draw(Mesh mesh,Material material)
        {

        }

        public void Render(GraphicsContext graphics)
        {
        }
    }
}
