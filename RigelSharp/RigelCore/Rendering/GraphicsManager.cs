using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Windows;
using SharpDX;

using RigelCore.Engine;

namespace RigelCore.Rendering
{
    public class GraphicsManager
    {
        private bool m_inited = false;
        private GraphicsContext m_context;
        private bool m_windowed = true;


        public void Init(RenderForm form)
        {
            if (m_inited) return;
            m_inited = true;
            m_context = new GraphicsContext();
            m_context.CreateWithSwapChain(form.Handle, form.Size.Width, form.Size.Height, m_windowed, true);


            TestInitDraw();
        }


        public void OnFrame()
        {
            m_context.Render(ImmediateDraw);
        }

        private void TestInitDraw()
        {

        }

        private void ImmediateDraw()
        {
            Graphics.DrawImmediate(null, null, Vector4.Zero,Quaternion.Identity,Vector3.One);
        }

        private void ProcessFullScreenSwitch(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Return && e.Alt)
            {
                Console.WriteLine("switch fullscreen");
                m_windowed = !m_windowed;
                m_context.SwitchFullscreen(m_windowed);
            }
        }

    }
}
