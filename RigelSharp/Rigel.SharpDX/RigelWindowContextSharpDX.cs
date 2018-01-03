using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Context;

using SharpDX;
using SharpDX.Windows;



namespace Rigel.SharpDX
{
    [RigelWindowContext(RigelSharpDX.CONTEXT_NAME, typeof(RigelWindowContextSharpDX), PlatformEnum.Windows)]
    public class RigelWindowContextSharpDX : IRigelWindowContext
    {
        private RigelSharpDXWindow m_window;

        public IRigelWindow MainWindow
        {
            get
            {
                return m_window;
            }
        }

        public IRigelWindow InitMainWindow(string title,bool fullscreen = false)
        {
            m_window = new RigelSharpDXWindow(title);
            m_window.IsFullscreen = fullscreen;
            return m_window;
        }
    }

    public class RigelSharpDXWindow : RenderForm,IRigelWindow
    {
        public RigelSharpDXWindow(string title) : base(title)
        {

        }

        public bool AllowResizing { get { return this.AllowResizing; } set { this.AllowResizing = value; } }
        public bool Fullscreen { get { return this.IsFullscreen; } set { this.IsFullscreen = value; } }

        public void DestroyWindow()
        {
            this.Close();
            if(!this.IsDisposed)
                this.Dispose();
        }

        public void Run(Action update)
        {
            if(update == null)
            {
                throw new Exception("Update Function is null");
            }
            RenderLoop.Run(this,()=> {
                update();
            });
        }
    }


}
