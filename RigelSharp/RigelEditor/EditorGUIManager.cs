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

using RigelEditor.Platforms;

namespace RigelEditor.EGUI
{

    public partial class EditorGUIManager:IDisposable,IEditorModule
    {
        private RenderForm m_form;
        private FontInfo m_font = null;
        private bool m_lastFrameDrag = false;
        private RigelCore.Vector2 m_LastPointerDrag;

        private GUIDockMgr m_dockMgr;

        
        internal FontInfo Font { get { return m_font; } }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }

        public RenderForm Form { get { return m_form; } }
        public GUIDockMgr DockManager { get { return m_dockMgr; } }


        private GUIEventHandlerSharpDX m_eventHandler;
        private GUIGraphicsBindSharpDX m_graphicsBind;
        internal IGUIGraphicsBind GraphicsBind { get { return m_graphicsBind; } }

        public EditorGUIManager()
        {

        }

        public void Init()
        {
            //basis
            m_form = RigelEditorApp.Instance.Form;
            

            m_dockMgr = new GUIDockMgr();

            m_eventHandler = new GUIEventHandlerSharpDX();
            m_eventHandler.RegisterEvent(m_form);

            ClientWidth = m_form.Width;
            ClientHeight = m_form.Height;

            //m_windows = new List<RigelEGUIWindow>();

            //m_mainMenu = new RigelEGUIMenu();
            //RefreshMainMenu();

            //m_dockerMgr = new RigelEGUIDockerManager();

            //RigelEGUI.InternalResetContext(this);


            m_graphicsBind = new GUIGraphicsBindSharpDX(EditorGraphicsManager.Instance.Graphics);
            m_font = new FontInfo("arial.ttf");
            m_graphicsBind.CrateFontTexture(m_font);

            GUIInternal.Init(m_eventHandler,m_graphicsBind,m_font);
        }


        private void GUIRelease()
        {
            //m_windows.Clear();
            //m_mainMenu.ClearAllItems();

            GUIInternal.Release();
        }


        public void Update()
        {
            m_graphicsBind.Update();
        }

        public void Dispose()
        {
            m_eventHandler.UnRegister();
            m_eventHandler = null;

            if(m_graphicsBind != null) m_graphicsBind.Dispose();
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
