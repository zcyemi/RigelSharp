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

        internal const float GUI_CLIP_PLANE_FAR = 100.0f;
        internal const float GUI_CLIP_PLANE_NEAR = 0;

        //shared indices buffer
        private RigelEGUIBuffer<int> m_bufferDataIndices;

        //window buffer
        private RigelEGUIBufferGUIWindow<RigelEGUIVertex> m_bufferDataRect;
        private RigelEGUIBufferGUIWindow<RigelEGUIVertex> m_bufferDataText;
        public RigelEGUIBufferGUIWindow<RigelEGUIVertex> BufferVertexRect { get { return m_bufferDataRect; } }
        public RigelEGUIBufferGUIWindow<RigelEGUIVertex> BufferVertexText { get { return m_bufferDataText; } }

        //main buffer
        private RigelEGUIBuffer<RigelEGUIVertex> m_bufferMainRect;
        private RigelEGUIBuffer<RigelEGUIVertex> m_bufferMainText;
        public RigelEGUIBuffer<RigelEGUIVertex> BufferMainRect { get { return m_bufferMainRect; } }
        public RigelEGUIBuffer<RigelEGUIVertex> BufferMainText { get { return m_bufferMainText; } }

        private Matrix m_matrixgui;
        private bool m_guiparamsChanged = true;


        private RigelGraphics m_graphics = null;

        private VertexShader m_gShaderVertex = null;
        private PixelShader m_gShaderPixelFont = null;
        private PixelShader m_gShaderPixelRect = null;
        private InputLayout m_gInputlayout = null;

        //buffer rect
        private Buffer m_gVertBufferWindowRect = null;
        private VertexBufferBinding m_gVertBufferWindowRectBinding;
        private Buffer m_gVertBufferWindowText = null;
        private VertexBufferBinding m_gVertBufferWindowTextBinding;

        private Buffer m_gVertBufferMainRect = null;
        private VertexBufferBinding m_gVertBufferMainRectBinding;
        private Buffer m_gVertBufferMainText = null;
        private VertexBufferBinding m_gVertBufferMainTextBinding;


        private Buffer m_gConstBuffer = null;
        private Buffer m_gIndicesBuffer = null;

        private Texture2D m_gFontTexture = null;
        private ShaderResourceView m_gFontTextureView = null;
        private SamplerState m_gFontTextureSampler = null;
        private BlendState m_gFontBlendState = null;


        private DeviceContext m_gDeferredContext = null;
        private CommandList m_gCommandlist = null;



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
            m_gDeferredContext = new DeviceContext(m_graphics.Device);

            //shaders
            var vshaderbc = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_FONT, "VS", "vs_4_0");
            var pshaderbcFont = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_FONT, "PS", "ps_4_0");
            var pshaderbcRect = ShaderBytecode.CompileFromFile(SHADER_FILE_PATH_RECT, "PS", "ps_4_0");

            m_gShaderVertex = new VertexShader(m_graphics.Device, vshaderbc);
            m_gShaderPixelFont = new PixelShader(m_graphics.Device, pshaderbcFont);
            m_gShaderPixelRect = new PixelShader(m_graphics.Device, pshaderbcRect);

            var signature = ShaderSignature.GetInputSignature(vshaderbc);
            m_gInputlayout = new InputLayout(m_graphics.Device, signature, new[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD",0,Format.R32G32_Float,32,0),
            });

            vshaderbc.Dispose();
            pshaderbcFont.Dispose();
            pshaderbcRect.Dispose();
            signature.Dispose();

            //blendstate
            {
                var blenddesc = BlendStateDescription.Default();
                blenddesc.RenderTarget[0].IsBlendEnabled = true;
                blenddesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                blenddesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
                blenddesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                blenddesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                blenddesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
                blenddesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                blenddesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

                m_gFontBlendState = new BlendState(m_graphics.Device, blenddesc);
            }
            



            //buffers
            //vertexbuffer window
            m_bufferDataRect = new RigelEGUIBufferGUIWindow<RigelEGUIVertex>(1024);
            var vbufferdescRect = new BufferDescription()
            {
                SizeInBytes = m_bufferDataRect.BufferSizeInByte,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            };

            m_gVertBufferWindowRect = new Buffer(m_graphics.Device, vbufferdescRect);
            m_gVertBufferWindowRectBinding = new VertexBufferBinding(
                m_gVertBufferWindowRect, 
                m_bufferDataRect.ItemSizeInByte,
                0
            );
            //textbuffer window 
            m_bufferDataText = new RigelEGUIBufferGUIWindow<RigelEGUIVertex>(1024);
            var vbufferdescText = new BufferDescription()
            {
                SizeInBytes = m_bufferDataRect.BufferSizeInByte,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            };
            m_gVertBufferWindowText = new Buffer(m_graphics.Device, vbufferdescText);
            m_gVertBufferWindowTextBinding = new VertexBufferBinding(
                m_gVertBufferWindowText, 
                m_bufferDataText.ItemSizeInByte, 
                0
            );

            //vertexbuffer main
            m_bufferMainRect = new RigelEGUIBufferGUIWindow<RigelEGUIVertex>(256);
            m_gVertBufferMainRect = new Buffer(m_graphics.Device, vbufferdescRect);
            m_gVertBufferMainRectBinding = new VertexBufferBinding(
                m_gVertBufferMainRect,
                m_bufferMainRect.ItemSizeInByte,
                0
            );
            //textbuffer main
            m_bufferMainText = new RigelEGUIBuffer<RigelEGUIVertex>(256);
            m_gVertBufferMainText = new Buffer(m_graphics.Device, vbufferdescText);
            m_gVertBufferMainTextBinding = new VertexBufferBinding(
                m_gVertBufferMainText,
                m_bufferMainText.ItemSizeInByte,
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

            m_matrixgui = Matrix.OrthoOffCenterLH(0, 800, 600, 0, GUI_CLIP_PLANE_NEAR, GUI_CLIP_PLANE_FAR);
            m_matrixgui.Transpose();
            m_gConstBuffer = Buffer.Create(m_graphics.Device, ref m_matrixgui, cbufferdesc);

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

            m_gIndicesBuffer = new Buffer(m_graphics.Device, ibufferdesc);

        }

        public void UpdateGUIParams(int width,int height)
        {
            m_matrixgui = Matrix.OrthoOffCenterLH(0,width, height, 0, GUI_CLIP_PLANE_NEAR, GUI_CLIP_PLANE_FAR);
            m_matrixgui.Transpose();

            m_guiparamsChanged = true;
        }

        public void CrateFontTexture(RigelFont font)
        {
            if (m_gFontTexture != null) throw new Exception("font texture2d already created");

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
                m_gFontTexture = new Texture2D(m_graphics.Device, desc, new DataRectangle(pinnedptr.AddrOfPinnedObject(), img.Pitch));
                pinnedptr.Free();
            }

            m_gFontTextureView = new ShaderResourceView(m_graphics.Device,m_gFontTexture);
            m_gFontTextureSampler = new SamplerState(m_graphics.Device,SamplerStateDescription.Default());
            
        }

        private void ReleaseCommandList()
        {
            if(m_gCommandlist != null) {
                m_gCommandlist.Dispose();
                m_gCommandlist = null;
            }
        }

        private void BuildCommandList()
        {
            m_gDeferredContext.OutputMerger.SetRenderTargets(m_graphics.DepthStencilViewDefault, m_graphics.RenderTargetViewDefault);
            m_gDeferredContext.Rasterizer.SetViewport(m_graphics.ViewPortDefault);
            
            //draw rects
            m_gDeferredContext.InputAssembler.InputLayout = m_gInputlayout;
            m_gDeferredContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            m_gDeferredContext.InputAssembler.SetVertexBuffers(0, m_gVertBufferWindowRectBinding);
            m_gDeferredContext.InputAssembler.SetIndexBuffer(m_gIndicesBuffer, Format.R32_UInt, 0);

            m_gDeferredContext.VertexShader.SetConstantBuffer(0, m_gConstBuffer);
            m_gDeferredContext.VertexShader.Set(m_gShaderVertex);

            m_gDeferredContext.PixelShader.Set(m_gShaderPixelRect);

            RigelUtility.Log("buffer data count:" + m_bufferDataRect.BufferDataCount);
            int indexedCount = m_bufferDataRect.BufferDataCount / 2 * 3;
            m_gDeferredContext.DrawIndexed(indexedCount, 0, 0);

            //draw text
            m_gDeferredContext.InputAssembler.SetVertexBuffers(0, m_gVertBufferWindowTextBinding);
            m_gDeferredContext.PixelShader.Set(m_gShaderPixelFont);
            m_gDeferredContext.PixelShader.SetShaderResource(0, m_gFontTextureView);
            m_gDeferredContext.PixelShader.SetSampler(0, m_gFontTextureSampler);

            m_gDeferredContext.OutputMerger.SetBlendState(m_gFontBlendState);

            int textIndexedCount = m_bufferDataText.BufferDataCount / 2 * 3;
            m_gDeferredContext.DrawIndexed(textIndexedCount, 0, 0);

            m_gCommandlist = m_gDeferredContext.FinishCommandList(false);

        }

        public void Render(RigelGraphics graphics)
        {
            if (m_graphics != graphics) throw new Exception("RigelGraphics not match!");

            //bufferData
            if (m_guiparamsChanged)
            {
                m_graphics.ImmediateContext.UpdateSubresource(ref m_matrixgui, m_gConstBuffer);
                m_guiparamsChanged = false;
            }


            if (m_bufferDataRect.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataRect.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject()
                }, m_gVertBufferWindowRect, 0);
                pinnedptr.Free();

                m_bufferDataRect.SetDirty(false);

                RigelUtility.Log("update vertexbuffer rect data");
            }

            if (m_bufferDataText.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataText.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject()
                }, m_gVertBufferWindowText, 0);
                pinnedptr.Free();

                m_bufferDataRect.SetDirty(false);

                RigelUtility.Log("update vertexbuffer text data");
            }

            if (m_bufferDataIndices.Dirty)
            {
                var pinnedptr = GCHandle.Alloc(m_bufferDataIndices.BufferData, GCHandleType.Pinned);
                m_graphics.ImmediateContext.UpdateSubresource(new DataBox()
                {
                    DataPointer = pinnedptr.AddrOfPinnedObject(),
                    RowPitch = 0,
                    SlicePitch = m_bufferDataIndices.ItemSizeInByte,
                }, m_gIndicesBuffer, 0);
                pinnedptr.Free();

                m_bufferDataIndices.SetDirty(false);
                RigelUtility.Log("update indicesbuffer data");
            }

            if (graphics.NeedRebuildCommandList || NeedRebuildCommandList)
            {
                if(m_gCommandlist != null)
                {
                    m_gCommandlist.Dispose();
                }
                BuildCommandList();

                NeedRebuildCommandList = false;
            }

            graphics.ImmediateContext.ExecuteCommandList(m_gCommandlist,false);


        }

        public void Dispose()
        {
            if (m_gShaderPixelRect != null) m_gShaderPixelRect.Dispose();
            if( m_gShaderPixelFont != null) m_gShaderPixelFont.Dispose();
            if (m_gShaderVertex != null) m_gShaderVertex.Dispose();
            if (m_gInputlayout != null) m_gInputlayout.Dispose();


            if (m_gVertBufferWindowRect != null) m_gVertBufferWindowRect.Dispose();
            if (m_gVertBufferWindowText != null) m_gVertBufferWindowText.Dispose();

            if (m_gVertBufferMainRect != null) m_gVertBufferMainRect.Dispose();
            if (m_gVertBufferMainText != null) m_gVertBufferMainText.Dispose();

            if (m_gConstBuffer != null) m_gConstBuffer.Dispose();
            if (m_gIndicesBuffer != null) m_gIndicesBuffer.Dispose();

            if(m_gFontTextureView != null) m_gFontTextureView.Dispose();
            if(m_gFontTexture != null) m_gFontTexture.Dispose();
            if(m_gFontTextureSampler != null) m_gFontTextureSampler.Dispose();
            if (m_gFontBlendState != null) m_gFontBlendState.Dispose();

            if (m_gCommandlist != null) m_gCommandlist.Dispose();
            if (m_gDeferredContext != null) m_gDeferredContext.Dispose();

            m_graphics = null;
        }
    }
}
