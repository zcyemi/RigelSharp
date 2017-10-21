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

using Buffer = SharpDX.Direct3D11.Buffer;

namespace RigelEditor
{
    public class RigelDrawCall
    {
        public VertexShader ShaderVertex { get; set; }
        public PixelShader ShaderPixel { get; set; }

        public InputLayout Layout { get; set; }
        public VertexBufferBinding? VertexBufferBinding;

        public PrimitiveTopology? InputPrimitiveTopology;
        public int DrawVertexCount = 0;
    }
}
