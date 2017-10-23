using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
    public partial class RigelEGUICtx:IDisposable
    {
        private RenderForm m_form;
        private RigelEGUIGraphicsBind m_graphicsBind = null;
        private RigelFont m_font = null;

        internal RigelEGUIGraphicsBind GraphicsBind { get { return m_graphicsBind; } }

        public RigelEGUICtx(RenderForm form,RigelGraphics graphics)
        {
            //basis
            m_form = form;
            m_graphicsBind = new RigelEGUIGraphicsBind(graphics);
            m_font = new RigelFont("arial.ttf");
            m_graphicsBind.CrateFontTexture(m_font);

            GUIInit();

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            m_form.UserResized += (sender, e) => {
                m_graphicsBind.UpdateGUIParams(m_form.ClientSize.Width, m_form.ClientSize.Height);
                RigelUtility.Log("event resize");
            };
            m_form.KeyDown += OnWindowEvent;
            m_form.KeyUp += OnWindowEvent;
            m_form.KeyPress += OnWindowEvent;
            m_form.MouseDown += OnWindowEvent;
            m_form.MouseUp += OnWindowEvent;
            m_form.MouseClick += OnWindowEvent;
            m_form.MouseDoubleClick += OnWindowEvent;
            m_form.MouseWheel += OnWindowEvent;
            m_form.DragEnter += OnWindowEvent;
            m_form.DragDrop += OnWindowEvent;

        }

        private void OnWindowEvent(object sender, EventArgs e)
        {
            GUIUpdate();
        }

        public void Render(RigelGraphics graphics)
        {
            m_graphicsBind.Render(graphics);
        }

        public void Dispose()
        {
            m_graphicsBind.Dispose();
            m_form = null;
        }


        internal T FindWindowOfType<T>() where T : RigelEditorGUIWindow,new()
        {
            foreach(var w in m_windows)
            {
                if(w is T)
                {
                    return w as T;
                }
            }
            T win = new T();
            m_windows.Add(win);

            return win;
        }

    }



}
