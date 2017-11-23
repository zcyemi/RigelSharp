using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;

using RigelCore.Rendering;


namespace RigelCore.Engine
{
    public static class GameEngine
    {
        private static RenderForm s_form;

        private static bool s_inited = false;

        private static GraphicsContext s_graphicsCtx;

        private static bool s_windowed = true;


        public static void Run()
        {
            if (s_inited) return;
            s_inited = true;


            s_form = new RenderForm("RigelGame");
            s_form.KeyUp += ProcessFullScreenSwitch;
            s_form.IsFullscreen = true;
            s_form.Show();

            s_graphicsCtx = new GraphicsContext();
            s_graphicsCtx.CreateWithSwapChain(s_form.Handle, s_form.Size.Width, s_form.Size.Height, s_windowed, true);

            RenderLoop.Run(s_form, s_graphicsCtx.Render);
        }

        private static void ProcessFullScreenSwitch(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == System.Windows.Forms.Keys.Return && e.Alt)
            {
                Console.WriteLine("switch fullscreen");
                s_windowed = !s_windowed;
                s_graphicsCtx.SwitchFullscreen(s_windowed);
            }
        }

        public static void Exit()
        {

        }
    }
}
