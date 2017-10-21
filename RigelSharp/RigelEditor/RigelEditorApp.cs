using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace RigelEditor
{
    public class RigelEditorApp
    {

        private RenderForm m_windowForm;
        private RigelGraphics m_graphics;


        private VertexShader shaderVertex;
        private PixelShader shaderPixel;

        private InputLayout layout = null;
        private SharpDX.Direct3D11.Buffer buffer;
        private SharpDX.Direct3D11.Buffer cbuffer;

        private Buffer m_indicesBuffer;

        private RigelDrawCall m_drawcall;

        public RigelEditorApp()
        {
            m_windowForm = new RenderForm("Rigel");
            m_windowForm.AllowUserResizing = true;

            m_graphics = new RigelGraphics();
            m_graphics.CreateWithSwapChain(m_windowForm);


            var vertexshaderbytecode = ShaderBytecode.CompileFromFile("MiniCube.fx", "VS", "vs_4_0");
            shaderVertex = new VertexShader(m_graphics.Device, vertexshaderbytecode);
            var pixelshaderbytecode = ShaderBytecode.CompileFromFile("MiniCube.fx", "PS", "ps_4_0");
            shaderPixel = new PixelShader(m_graphics.Device, pixelshaderbytecode);

            var signature = ShaderSignature.GetInputSignature(vertexshaderbytecode);
            layout = new InputLayout(m_graphics.Device, signature, new[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
            });

            vertexshaderbytecode.Dispose();
            pixelshaderbytecode.Dispose();
            signature.Dispose();

            m_indicesBuffer = Buffer.Create(m_graphics.Device, BindFlags.IndexBuffer, new int[]{
                0,2,1,0,3,2
            });

            buffer = SharpDX.Direct3D11.Buffer.Create(m_graphics.Device, BindFlags.VertexBuffer, new[]
            {
                new Vector4(0,0,0,1), Vector4.One,
                new Vector4(0,100,0,1),Vector4.Zero,
                new Vector4(100,100,0,1),Vector4.One,

                new Vector4(100,0,0,1),Vector4.One,
            });

            cbuffer = new Buffer(m_graphics.Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);


            m_drawcall = new RigelDrawCall()
            {
                ShaderPixel = shaderPixel,
                ShaderVertex = shaderVertex,
                DrawVertexCount = 36,
                Layout = layout,
                InputPrimitiveTopology = PrimitiveTopology.TriangleList,
                VertexBufferBinding = new VertexBufferBinding(buffer, Utilities.SizeOf<Vector4>() * 2, 0)
            };

            m_graphics.ImmediateContext.VertexShader.SetConstantBuffer(0, cbuffer);

            m_graphics.ExecRenderCall(m_drawcall);

            m_graphics.ImmediateContext.InputAssembler.SetIndexBuffer(m_indicesBuffer, Format.R32_UInt, 0);
        }

        public void EnterRunloop() {

            var view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.Identity;

            // Use clock
            var clock = new Stopwatch();
            clock.Start();

            
            RenderLoop.Run(m_windowForm, () =>
            {
                proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, m_windowForm.ClientSize.Width / (float)m_windowForm.ClientSize.Height, 0.1f, 100.0f);

                var time = clock.ElapsedMilliseconds / 1000.0f;
                var viewProj = Matrix.Multiply(view, proj);

                Matrix orith = Matrix.OrthoOffCenterRH(0,m_windowForm.ClientSize.Width,m_windowForm.ClientSize.Height,0, 0, 1.0f);
                orith.Transpose();

                m_graphics.Render(()=> {

                    var worldViewProj1 = Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
                    worldViewProj1.Transpose();
                    m_graphics.ImmediateContext.UpdateSubresource(ref orith, cbuffer);

                    //m_graphics.ImmediateContext.Draw(36, 0);
                    m_graphics.ImmediateContext.DrawIndexed(6, 0, 0);

                    
                });
            });


            shaderVertex.Dispose();
            shaderPixel.Dispose();

            layout.Dispose();
            buffer.Dispose();
            cbuffer.Dispose();

            m_indicesBuffer.Dispose();


            m_graphics.Dispose();
            m_windowForm.Dispose();
        }

    }
}
