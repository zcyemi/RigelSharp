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

    internal class RigelEGUIGraphicsBind:IDisposable
    {
        private static readonly string SHADER_FILE_PATH_FONT = "Shader/gui_font.fx";
        private static readonly string SHADER_FILE_PATH_RECT = "Shader/gui_rect.fx";
        private const int TEXTURE_FONT_SIZE = 128;


        private RigelEGUIBuffer<RigelEGUIVertex> m_bufferDataRect;
        private RigelEGUIBuffer<RigelEGUIVertex> m_bufferDataText;
        private RigelEGUIBuffer<int> m_bufferDataIndices;

        public RigelEGUIBuffer<RigelEGUIVertex> BufferVertexRect { get { return m_bufferDataRect; } }

        private Matrix m_matrixgui;
        private bool m_guiparamsChanged = true;


        private RigelGraphics m_graphics = null;

        private VertexShader m_shaderVertex = null;
        private PixelShader m_shaderPixelFont = null;
        private PixelShader m_shaderPixelRect = null;
        private InputLayout m_inputlayout = null;

        //buffer rect
        private Buffer m_vertexBufferRect = null;
        private VertexBufferBinding m_vertexBufferRectBinding;

        private Buffer m_vertexBuffertText = null;
        private VertexBufferBinding m_vertexBufferTextBinding;


        private Buffer m_constBuffer = null;
        private Buffer m_indicesBuffer = null;

        private Texture2D m_fontTexture = null;
        private ShaderResourceView m_fontTextureView = null;
        private SamplerState m_fontTextureSampler = null;


        private DeviceContext m_deferredContext = null;
        private CommandList m_commandlist = null;

        /// <summary>
        /// mark true when buffer is modified
        /// </summary>
        internal bool NeedRebuildCommandList = false;


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
            var vshaderbc = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_FONT, "VS", "vs_4_0");
            var pshaderbcFont = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_FONT, "PS", "ps_4_0");
            var pshaderbcRect = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_RECT, "PS", "ps_4_0");

            m_shaderVertex = new VertexShader(m_graphics.Device, vshaderbc);
            m_shaderPixelFont = new PixelShader(m_graphics.Device, pshaderbcFont);
            m_shaderPixelRect = new PixelShader(m_graphics.Device, pshaderbcRect);

            var signature = ShaderSignature.GetInputSignature(vshaderbc);
            m_inputlayout = new InputLayout(m_graphics.Device, signature, new[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD",0,Format.R32G32_Float,32,0),
            });

            vshaderbc.Dispose();
            pshaderbcFont.Dispose();
            pshaderbcRect.Dispose();
            signature.Dispose();

            //buffers

            //vertexbuffer

            var vbufferdesc = new BufferDescription()
            {
                SizeInBytes = m_bufferDataRect.BufferSizeInByte,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            };

            m_bufferDataRect = new RigelEGUIBuffer<RigelEGUIVertex>(1024);
            m_vertexBufferRect = new Buffer(m_graphics.Device, vbufferdesc);
            m_vertexBufferRectBinding = new VertexBufferBinding(
                m_vertexBufferRect, 
                m_bufferDataRect.ItemSizeInByte,
                0
            );

            m_bufferDataText = new RigelEGUIBuffer<RigelEGUIVertex>(1024);
            m_vertexBuffertText = new Buffer(m_graphics.Device, vbufferdesc);
            m_vertexBufferTextBinding = new VertexBufferBinding(
                m_vertexBuffertText, 
                m_bufferDataText.ItemSizeInByte, 
                0
            );

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

            m_deferredContext.InputAssembler.SetVertexBuffers(0, m_vertexBufferRectBinding);
            m_deferredContext.InputAssembler.SetIndexBuffer(m_indicesBuffer, Format.R32_UInt, 0);

            m_deferredContext.VertexShader.SetConstantBuffer(0, m_constBuffer);
            m_deferredContext.VertexShader.Set(m_shaderVertex);

            m_deferredContext.PixelShader.Set(m_shaderPixelRect);
            //m_deferredContext.PixelShader.SetShaderResource(0, m_fontTextureView);
            //m_deferredContext.PixelShader.SetSampler(0, m_fontTextureSampler);

            RigelUtility.Log("buffer data count:" + m_bufferDataRect.BufferDataCount);
            int indexedCount = m_bufferDataRect.BufferDataCount / 2 * 3;
            m_deferredContext.DrawIndexed(indexedCount, 0, 0);

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


            if (m_bufferDataRect.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataRect.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject()
                }, m_vertexBufferRect, 0);
                pinnedptr.Free();

                m_bufferDataRect.SetDirty(false);

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

            if (graphics.NeedRebuildCommandList || NeedRebuildCommandList)
            {
                if(m_commandlist != null)
                {
                    m_commandlist.Dispose();
                }
                BuildCommandList();

                NeedRebuildCommandList = false;
            }

            graphics.ImmediateContext.ExecuteCommandList(m_commandlist,false);


        }

        public void Dispose()
        {
            if (m_shaderPixelRect != null) m_shaderPixelRect.Dispose();
            if( m_shaderPixelFont != null) m_shaderPixelFont.Dispose();
            if (m_shaderVertex != null) m_shaderVertex.Dispose();
            if (m_inputlayout != null) m_inputlayout.Dispose();

            if (m_vertexBufferRect != null) m_vertexBufferRect.Dispose();
            if (m_vertexBuffertText != null) m_vertexBuffertText.Dispose();

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
