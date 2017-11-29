using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore.Rendering
{
    public enum GraphicsBufferType
    {
        Default,
        Dynamic
    };

    public class GraphicsBuffer
    {
        public GraphicsBufferType bufferType { get; private set; }

        public GraphicsBuffer(GraphicsBufferType bufferType = GraphicsBufferType.Default)
        {
            this.bufferType = bufferType;
        }
        
    }
}
