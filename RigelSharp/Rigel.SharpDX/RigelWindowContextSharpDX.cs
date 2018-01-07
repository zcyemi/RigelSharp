using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Context;

using SharpDX;
using SharpDX.Windows;

using System.Windows.Forms;


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
            m_eventHandler = new RigelSharpDXWindowEventHandler(this);
        }

        public bool AllowResizing { get { return this.AllowResizing; } set { this.AllowResizing = value; } }
        public bool Fullscreen { get { return this.IsFullscreen; } set { this.IsFullscreen = value; } }


        private RigelSharpDXWindowEventHandler m_eventHandler;

        public IRigelWindowEventHandler EvnetHandler
        {
            get
            {
                return m_eventHandler;
            }
        }

        public void DestroyWindow()
        {
            this.Close();


            if(m_eventHandler != null)
            {
                m_eventHandler.Dispose(this);
                m_eventHandler = null;
            }

            if(!this.IsDisposed)
                this.Dispose();
        }

        public IntPtr GetWindowHandler()
        {
            return this.Handle;
        }

        public int GetWindowHeight()
        {
            return this.ClientSize.Height;
        }

        public int GetWindowWidth()
        {
            return this.ClientSize.Width;
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

    public class RigelSharpDXWindowEventHandler : IRigelWindowEventHandler
    {
        public event EventHandler OnDragDrop = delegate { };
        public event EventHandler OnDragEnter = delegate { };
        public event EventHandler OnMouseWheel = delegate { };
        public event EventHandler OnMouseClick = delegate { };
        public event EventHandler OnMouseDoubleClick = delegate { };
        public event EventHandler OnMouseMove = delegate { };
        public event EventHandler OnMouseDown = delegate { };
        public event EventHandler OnMouseUp = delegate { };
        public event EventHandler OnKeyPress = delegate { };
        public event EventHandler OnKeyDown = delegate { };
        public event EventHandler OnKeyUp = delegate { };
        public event EventHandler OnUserResize;






        void handlerUserResize(object sender, EventArgs e) {
            OnUserResize.Invoke(sender,e);
        }

        void handlerKeyDown(object sender,KeyEventArgs e)
        {
            OnKeyDown.Invoke(sender, e);
        }

        void handlerKeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(sender, e);
        }

        void handlerKeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(sender, e);
        }

        void handlerMouseMove(object sender,MouseEventArgs e)
        {
            OnMouseMove(sender, e);
        }

        void handlerMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e);
        }

        void handlerMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(sender, e);
        }

        void handlerMouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(sender, e);
        }

        void handlerMouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDoubleClick(sender, e);
        }


        void handlerMouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(sender, e);
        }

        void handlerDragEnter(object sender,DragEventArgs e)
        {
            OnDragEnter(sender, e);
        }

        void handlerDragDrop(object sender, DragEventArgs e)
        {
            OnDragDrop(sender, e);
        }



        public RigelSharpDXWindowEventHandler(RenderForm form)
        {

            form.UserResized += handlerUserResize;
            form.KeyDown += handlerKeyDown;
            form.KeyUp += handlerKeyUp;
            form.KeyPress += handlerKeyPress;
            form.MouseMove += handlerMouseMove;
            form.MouseDown += handlerMouseDown;
            form.MouseUp += handlerMouseUp;
            form.MouseClick += handlerMouseClick;
            form.MouseDoubleClick += handlerMouseDoubleClick;
            form.MouseWheel += handlerMouseWheel;
            form.DragEnter += handlerDragEnter;
            form.DragDrop += handlerDragDrop;
        }


        private void CreateGeneralEvent()
        {
        }

        public void Dispose(RenderForm form)
        {
            if (form == null) return;
            form.UserResized -= handlerUserResize;
            form.KeyDown -= handlerKeyDown;
            form.KeyUp -= handlerKeyUp;
            form.KeyPress -= handlerKeyPress;
            form.MouseMove -= handlerMouseMove;
            form.MouseDown -= handlerMouseDown;
            form.MouseUp -= handlerMouseUp;
            form.MouseClick -= handlerMouseClick;
            form.MouseDoubleClick -= handlerMouseDoubleClick;
            form.MouseWheel -= handlerMouseWheel;
            form.DragEnter -= handlerDragEnter;
            form.DragDrop -= handlerDragDrop;
        }
    }


}
