using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Rigel;
using Rigel.Engine;

using SharpDX.Direct3D11;


namespace Rigel.Rendering
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

    internal enum CommandDataType
    {
        ClearRenderTarget,
        Draw,
    }

    internal struct CommandBufferData
    {
        public CommandDataType Type;

        public Vector4 ParamVec4;
        public RenderTextureIdentifier ParamRTId;
    }

    public class CommandBuffer:ICommandBuffer
    {
        private CommandList m_commandList = null;
        private List<CommandBufferData> m_commandData = new List<CommandBufferData>();

        public CommandBuffer()
        {

        }


        public void ClearRenderTarget(RenderTextureIdentifier texture,Vector4 color)
        {
            m_commandData.Add(new CommandBufferData()
            {
                Type = CommandDataType.ClearRenderTarget,
                ParamRTId = texture,
                ParamVec4 = color,
            });
        }

        public void Dispose()
        {
            if(m_commandList != null)
                m_commandList.Dispose();
            m_commandList = null;
        }

        public void Draw(Mesh mesh,Material material)
        {

        }

        public void Render(GraphicsContext graphics)
        {
            if(m_commandList == null)
            {
                InternalRebuildCommandList(graphics);
            }

            if(m_commandList != null)
            {
                graphics.ImmediateContext.ExecuteCommandList(m_commandList, false);
            }
        }

        public void ClearAllCommand()
        {
            InternalReleaseCommandList();

            m_commandData.Clear();
        }

        internal void InternalReleaseCommandList()
        {
            if (m_commandList != null)
            {
                m_commandList.Dispose();
                m_commandList = null;
            }
        }

        internal void InternalRebuildCommandList(GraphicsContext context)
        {
            if (m_commandData.Count == 0) return;

            var deferred = new DeviceContext(context.Device);

            foreach (var cmd in m_commandData)
            {
                switch (cmd.Type)
                {
                    case CommandDataType.ClearRenderTarget:

                        deferred.ClearRenderTargetView(cmd.ParamRTId.GetRawRenderTexture<RenderTargetView>(context),new SharpDX.Color(0,0,0,0));
                        break;
                    case CommandDataType.Draw:

                        break;
                }
            }

            m_commandList =  deferred.FinishCommandList(false);
            deferred.Dispose();
            deferred = null;
        }

    }


}
