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
using RigelCore;

namespace RigelEditor.EGUI
{

    public partial class EditorGUICtx:IDisposable
    {
        private RenderForm m_form;
        private GUIGraphicsBind m_graphicsBind = null;
        private FontInfo m_font = null;
        private bool m_lastFrameDrag = false;
        private Vector2 m_LastPointerDrag;

        private GUIDockMgr m_dockMgr;

        internal GUIGraphicsBind GraphicsBind { get { return m_graphicsBind; } }
        internal FontInfo Font { get { return m_font; } }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }

        public RenderForm Form { get { return m_form; } }
        public GUIDockMgr DockManager { get { return m_dockMgr; } }

        public EditorGUICtx(RenderForm form,EditorGraphicsManager graphics)
        {

            //basis
            m_form = form;
            m_graphicsBind = new GUIGraphicsBind(graphics);
            m_font = new FontInfo("arial.ttf");
            m_graphicsBind.CrateFontTexture(m_font);

            m_dockMgr = new GUIDockMgr();

            GUIInit();

            RegisterEvent();
        }

        

        private void OnWindowEvent(GUIEvent guievent)
        {
            if(guievent.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if(m_lastFrameDrag == false)
                {
                    guievent.EventType = RigelEGUIEventType.MouseDragEnter;
                    m_lastFrameDrag = true;
                }
            }
            else if (m_lastFrameDrag == true && (guievent.EventType & RigelEGUIEventType.MouseEvent) > 0)
            {
                m_lastFrameDrag = false;
                guievent.EventType = RigelEGUIEventType.MouseDragLeave;
            }

            if (guievent.IsMouseDragEvent())
            {
                if(guievent.EventType == RigelEGUIEventType.MouseDragUpdate)
                {
                    guievent.DragOffset = guievent.Pointer - m_LastPointerDrag;
                }
                m_LastPointerDrag = guievent.Pointer;
            }
            GUIUpdate(guievent);

        }

        public void Render(EditorGraphicsManager graphics)
        {
            m_graphicsBind.Render(graphics);
        }

        private void GUIInit()
        {
            ClientWidth = m_form.Width;
            ClientHeight = m_form.Height;

            //m_windows = new List<RigelEGUIWindow>();

            //m_mainMenu = new RigelEGUIMenu();
            //RefreshMainMenu();

            //m_dockerMgr = new RigelEGUIDockerManager();

            //RigelEGUI.InternalResetContext(this);

            GUIInternal.Init(this);

        }
        private void GUIRelease()
        {
            //m_windows.Clear();
            //m_mainMenu.ClearAllItems();

            GUIInternal.Release();
        }
        private void GUIUpdate(GUIEvent guievent)
        {

            GUIInternal.Update(guievent);

            //RigelUtility.Log("------------- New Frame -----------");
            //RigelEGUI.InternalFrameBegin(guievent);

            //GUIUpdateMainBegin(guievent);
            ////GUIUpdateWindow(guievent);
            //GUIUpdateMainEnd(guievent);


            ////draw end
            //RigelEGUI.InternalFrameEnd();

        }

        public void Dispose()
        {
            m_graphicsBind.Dispose();
            m_form = null;
        }

        
        //internal T FindWindowOfType<T>() where T : RigelEGUIWindow,new()
        //{
        //    foreach(var w in m_windows)
        //    {
        //        if(w is T)
        //        {
        //            return w as T;
        //        }
        //    }
        //    T win = new T();
        //    m_windows.Add(win);

        //    return win;
        //}
        //private void RefreshMainMenu()
        //{
        //    var assembly = RigelReflectionHelper.AssemblyRigelEditor;
        //    foreach (var type in assembly.GetTypes())
        //    {
        //        var methods = RigelReflectionHelper.GetMethodByAttribute<RigelEGUIMenuItemAttribute>(
        //            type,
        //            BindingFlags.Static |
        //            BindingFlags.Public |
        //            BindingFlags.NonPublic
        //        );

        //        foreach (var m in methods)
        //        {
        //            var attr = Attribute.GetCustomAttribute(m, typeof(RigelEGUIMenuItemAttribute)) as RigelEGUIMenuItemAttribute;
        //            m_mainMenu.AddMenuItem(attr.Label, m);
        //        }

        //    }
        //    RigelUtility.Log("EGUI mainMenu item count:" + m_mainMenu.ItemNodes.Count());

        //}

    }



}
