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

using RigelCore;
using RigelEditor.EGUI;

namespace RigelEditor
{
    public class RigelEditorApp :Singleton<RigelEditorApp>
    {
        private String m_title = "Rigel";

        private RenderForm m_windowForm;

        public RenderForm Form { get { return m_windowForm; } }
        public EditorGUICtx EditorGUI { get; private set; }


        public RigelEditorApp()
        {

        }

        public void Init()
        {
            m_windowForm = new RenderForm(m_title);
            m_windowForm.AllowUserResizing = true;


            EditorGraphicsManager.Instance.Init();
            //test
            //var testwind = RigelEGUIWindow.GetWindow<RigelEditorAboutPage>();
            //var consolewin = RigelEGUIWindow.GetWindow<RigelEditorConsoleWindow>();

            EditorModuleManager.Instance.Init();
            EditorGUI = EditorModuleManager.Instance.FindModule<EditorGUICtx>();
        }

        public void EnterRunloop() {

            RenderLoop.Run(m_windowForm, () =>
            {
                EditorModuleManager.Instance.Update();

                EditorGraphicsManager.Instance.Render(() =>
                {
                });
            });

            EditorModuleManager.Instance.Dispose();
            EditorGraphicsManager.Instance.Dispose();

            m_windowForm.Dispose();
        }
        
        internal void SetCaptionInfo(string content)
        {
            m_windowForm.Text = string.Format("{0} - {1}", m_title, content);
        }

    }
}
