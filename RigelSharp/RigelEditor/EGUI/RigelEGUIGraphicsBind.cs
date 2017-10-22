using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

using RigelEditor;

namespace RigelEditor.EGUI
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RigelEGUIVertex
    {
        [FieldOffset(0)]
        public Vector4 Position;
        [FieldOffset(16)]
        public Vector4 Color;
        [FieldOffset(32)]
        public Vector2 UV;
    }

    public class RigelEGUIBuffer<T> where T:struct
    {
        private const int BUFFER_INIT_SIZE = 1024;
        private const int BUFFER_EXTEND_TIMES = 2;

        public int BufferSize { get { return BufferData.Length; } }
        public int BufferSizeInByte { get { return BufferSize * ItemSizeInByte; } }
        public int ItemSizeInByte { get { return RigelUtility.SizeOf<T>(); } }
        public bool BufferResized { get; private set; }

        private int m_bufferExtendTimes = BUFFER_EXTEND_TIMES;
        private Action<RigelEGUIBuffer<T>,int> m_extendGenrateFunc = null;

        public int BufferDataCount { get; private set; }
        public bool Dirty { get; private set; }

        public T[] BufferData;


        public RigelEGUIBuffer(int buffersize = BUFFER_INIT_SIZE,int extendtimes = BUFFER_EXTEND_TIMES,Action<RigelEGUIBuffer<T>,int> extendGenerate = null)
        {
            m_bufferExtendTimes = extendtimes;
            BufferDataCount = 0;
            BufferResized = false;
            m_extendGenrateFunc = extendGenerate;

            BufferData = new T[buffersize];
            if (m_extendGenrateFunc != null) m_extendGenrateFunc.Invoke(this,0);
        }

        public void IncreaseBufferDataCount(int c = 1)
        {
            BufferDataCount += c;
            Dirty = true;
        }

        public void CheckAndExtends(int tolerance = 16)
        {
            if(BufferDataCount > BufferSize - tolerance)
            {
                int extendpos = BufferSize;
                Array.Resize(ref BufferData, BufferSize * m_bufferExtendTimes);
                BufferResized = true;
                if (m_extendGenrateFunc != null) m_extendGenrateFunc.Invoke(this,extendpos);
            }
        }

        public void SetDirty(bool dirty)
        {
            Dirty = dirty;
        }

    }

    internal class RigelEGUIGraphicsBind:IDisposable
    {
        private static readonly string SHADER_FILE_PATH = "Shader/MiniCube.fx";
        private const int TEXTURE_FONT_SIZE = 128;


        private RigelEGUIBuffer<RigelEGUIVertex> m_bufferDataVertex;
        private RigelEGUIBuffer<int> m_bufferDataIndices;

        private Matrix m_matrixgui;
        private bool m_guiparamsChanged = true;


        private RigelGraphics m_graphics = null;

        private VertexShader m_shaderVertex = null;
        private PixelShader m_shaderPixel = null;
        private InputLayout m_inputlayout = null;


        private Buffer m_vertexBuffer = null;
        private VertexBufferBinding m_vertexBufferBinding;
        private Buffer m_constBuffer = null;
        private Buffer m_indicesBuffer = null;

        private Texture2D m_fontTexture = null;
        private ShaderResourceView m_fontTextureView = null;
        private SamplerState m_fontTextureSampler = null;


        private DeviceContext m_deferredContext = null;
        private CommandList m_commandlist = null;


        public RigelEGUIGraphicsBind(RigelGraphics graphics)
        {
            m_graphics = graphics;

            graphics.EventReleaseCommandList += ReleaseCommandList;

            InitGraphicsObjects();
        }


        private void InitGraphicsObjects()
        {

            //deferred context;
            m_deferredContext = new DeviceContext(m_graphics.Device);

            //shaders
            var vshaderbc = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH, "VS", "vs_4_0");
            var pshaderbc = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH, "PS", "ps_4_0");

            m_shaderVertex = new VertexShader(m_graphics.Device, vshaderbc);
            m_shaderPixel = new PixelShader(m_graphics.Device, pshaderbc);

            var signature = ShaderSignature.GetInputSignature(vshaderbc);
            m_inputlayout = new InputLayout(m_graphics.Device, signature, new[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD",0,Format.R32G32_Float,32,0),
            });

            vshaderbc.Dispose();
            pshaderbc.Dispose();
            signature.Dispose();

            //buffers

            //vertexbuffer
            m_bufferDataVertex = new RigelEGUIBuffer<RigelEGUIVertex>(1024);
            m_bufferDataVertex.BufferData[0].Position = new Vector4(0, 0, 0, 1);
            m_bufferDataVertex.BufferData[1].Position = new Vector4(0, 100, 0, 1);
            m_bufferDataVertex.BufferData[2].Position = new Vector4(100, 100, 0, 1);
            m_bufferDataVertex.BufferData[3].Position = new Vector4(100, 0, 0, 1);

            m_bufferDataVertex.BufferData[0].Color = Vector4.One;
            m_bufferDataVertex.BufferData[1].Color = Vector4.Zero;
            m_bufferDataVertex.BufferData[2].Color = Vector4.One;
            m_bufferDataVertex.BufferData[3].Color = Vector4.One;

            m_bufferDataVertex.BufferData[0].UV = new Vector2(0,0);
            m_bufferDataVertex.BufferData[1].UV = new Vector2(0,1);
            m_bufferDataVertex.BufferData[2].UV = new Vector2(1,1);
            m_bufferDataVertex.BufferData[3].UV = new Vector2(1,0);

            m_bufferDataVertex.IncreaseBufferDataCount(4);

            var vbufferdesc = new BufferDescription()
            {
                SizeInBytes = m_bufferDataVertex.BufferSizeInByte,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            };

            m_vertexBuffer = new Buffer(m_graphics.Device, vbufferdesc);
            m_vertexBufferBinding = new VertexBufferBinding(m_vertexBuffer, m_bufferDataVertex.ItemSizeInByte, 0);

            //const buffer
            var cbufferdesc = new BufferDescription()
            {
                SizeInBytes = RigelUtility.SizeOf<Matrix>(),
                BindFlags = BindFlags.ConstantBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            };

            m_matrixgui = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1.0f);
            m_matrixgui.Transpose();
            m_constBuffer = Buffer.Create(m_graphics.Device, ref m_matrixgui, cbufferdesc);

            //indices buffer

            m_bufferDataIndices = new RigelEGUIBuffer<int>(1024,2,(b,pos)=> {
                int tric = pos / 6;
                int trie = b.BufferSize / 6;
                for(int i = tric; i < trie; i++)
                {
                    int i6 = i * 6;
                    int i4 = i * 4;
                    b.BufferData[i6] = i4;
                    b.BufferData[i6+1] = i4+2;
                    b.BufferData[i6+2] = i4+1;

                    b.BufferData[i6+3] = i4;
                    b.BufferData[i6+4] = i4+3;
                    b.BufferData[i6+5] = i4+2;
                }

                b.IncreaseBufferDataCount(b.BufferSize - pos);
            });

            var ibufferdesc = new BufferDescription()
            {
                SizeInBytes = m_bufferDataIndices.BufferSizeInByte,
                BindFlags = BindFlags.IndexBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = m_bufferDataIndices.ItemSizeInByte
            };

            m_indicesBuffer = new Buffer(m_graphics.Device, ibufferdesc);

        }

        public void UpdateGUIParams(int width,int height)
        {
            m_matrixgui = Matrix.OrthoOffCenterLH(0,width, height, 0, 0, 1.0f);
            m_matrixgui.Transpose();

            m_guiparamsChanged = true;
        }

        public void CrateFontTexture(RigelFont font)
        {
            if (m_fontTexture != null) throw new Exception("font texture2d already created");


            using (RigelImageData img = new RigelImageData(TEXTURE_FONT_SIZE, TEXTURE_FONT_SIZE))
            {
                font.GenerateFontTexture(img);
                var desc = new Texture2DDescription()
                {
                    Width = img.Width,
                    Height = img.Height,
                    ArraySize = 1,
                    SampleDescription = new SampleDescription(1,0),
                    Format = Format.R8G8B8A8_UNorm,
                    Usage = ResourceUsage.Default,
                    CpuAccessFlags = CpuAccessFlags.None,
                    BindFlags = BindFlags.ShaderResource,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                };

                var pinnedptr = System.Runtime.InteropServices.GCHandle.Alloc(img.Data, System.Runtime.InteropServices.GCHandleType.Pinned);
                m_fontTexture = new Texture2D(m_graphics.Device, desc, new DataRectangle(pinnedptr.AddrOfPinnedObject(), img.Pitch));
                pinnedptr.Free();
            }

            m_fontTextureView = new ShaderResourceView(m_graphics.Device,m_fontTexture);
            m_fontTextureSampler = new SamplerState(m_graphics.Device,SamplerStateDescription.Default());
            
        }

        private void ReleaseCommandList()
        {
            if(m_commandlist != null) {
                m_commandlist.Dispose();
                m_commandlist = null;
            }
        }

        private void BuildCommandList()
        {
            m_deferredContext.OutputMerger.SetRenderTargets(m_graphics.DepthStencilViewDefault, m_graphics.RenderTargetViewDefault);
            m_deferredContext.Rasterizer.SetViewport(m_graphics.ViewPortDefault);
            //draw rects

            m_deferredContext.InputAssembler.InputLayout = m_inputlayout;
            m_deferredContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            m_deferredContext.InputAssembler.SetVertexBuffers(0, m_vertexBufferBinding);
            m_deferredContext.InputAssembler.SetIndexBuffer(m_indicesBuffer, Format.R32_UInt, 0);

            m_deferredContext.VertexShader.SetConstantBuffer(0, m_constBuffer);
            m_deferredContext.VertexShader.Set(m_shaderVertex);

            m_deferredContext.PixelShader.Set(m_shaderPixel);
            m_deferredContext.PixelShader.SetShaderResource(0, m_fontTextureView);
            m_deferredContext.PixelShader.SetSampler(0, m_fontTextureSampler);

            m_deferredContext.DrawIndexed(6, 0, 0);

            m_commandlist = m_deferredContext.FinishCommandList(false);

        }

        public void Render(RigelGraphics graphics)
        {
            if (m_graphics != graphics) throw new Exception("RigelGraphics not match!");

            //bufferData
            if (m_guiparamsChanged)
            {
                m_graphics.ImmediateContext.UpdateSubresource(ref m_matrixgui, m_constBuffer);
                m_guiparamsChanged = false;
            }


            if (m_bufferDataVertex.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataVertex.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject()
                }, m_vertexBuffer, 0);
                pinnedptr.Free();

                m_bufferDataVertex.SetDirty(false);

                RigelUtility.Log("update vertexbuffer data");
            }

            if (m_bufferDataIndices.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataIndices.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject(),
                    RowPitch = 0,
                    SlicePitch = m_bufferDataIndices.ItemSizeInByte,
                }, m_indicesBuffer, 0);
                pinnedptr.Free();

                m_bufferDataIndices.SetDirty(false);
                RigelUtility.Log("update indicesbuffer data");
            }

            if (graphics.NeedRebuildCommandList)
            {
                if(m_commandlist != null)
                {
                    m_commandlist.Dispose();
                }
                BuildCommandList();
            }

            graphics.ImmediateContext.ExecuteCommandList(m_commandlist,false);


        }

        public void Dispose()
        {
            if( m_shaderPixel != null) m_shaderPixel.Dispose();
            if (m_shaderVertex != null) m_shaderVertex.Dispose();
            if (m_inputlayout != null) m_inputlayout.Dispose();
            if (m_vertexBuffer != null) m_vertexBuffer.Dispose();
            if (m_constBuffer != null) m_constBuffer.Dispose();
            if (m_indicesBuffer != null) m_indicesBuffer.Dispose();

            if(m_fontTextureView != null) m_fontTextureView.Dispose();
            if(m_fontTexture != null) m_fontTexture.Dispose();
            if(m_fontTextureSampler != null) m_fontTextureSampler.Dispose();

            if (m_commandlist != null) m_commandlist.Dispose();
            if (m_deferredContext != null) m_deferredContext.Dispose();

            m_graphics = null;
        }
    }
}
