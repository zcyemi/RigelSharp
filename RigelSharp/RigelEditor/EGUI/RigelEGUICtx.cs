﻿using System;
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

        

        private void OnWindowEvent(RigelEGUIEvent guievent)
        {
            GUIUpdate(guievent);
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


        internal T FindWindowOfType<T>() where T : RigelEGUIWindow,new()
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
