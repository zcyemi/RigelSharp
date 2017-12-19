using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rigel.EGUI
{
    class GUIDrawStageMain : GUIDrawStage
    {
        private bool m_initedDraw = false;
        private bool m_needUpdateBuffer = false;

        //private int m_lastBufferSizeRect = 0;
        //private int m_lastBufferSizeText = 0;

        public GUIDrawStageMain(string stagename, int order = 0) : base(stagename, order)
        {
        }

        public override void Draw(GUIEvent guievent)
        {
            base.Draw(guievent);

            m_needUpdateBuffer = false;
            if (guievent.Used && m_initedDraw) return; 

            m_drawTarget.bufferRect.Clear();
            m_drawTarget.bufferText.Clear();


            DrawContent();

            m_needUpdateBuffer = true;
        }

        public override void SyncBuffer(IGUIGraphicsBind guibind)
        {
            if (!m_needUpdateBuffer) return;

            var bind = guibind;

            bind.SyncDrawTarget(this,m_drawTarget);
        }




        private void DrawContent()
        {
            GUILayout.BeginToolBar(23);
            {
                GUILayout.Text("RIGEL");
                //EditorMenuManager.Instance.OnDrawMainMenuBar();
            }
            GUILayout.EndToolBar();


            var dockgroup = GUI.Context.currentGroup.Rect;
            dockgroup.Y = 23;
            dockgroup.W -= 23;


            //var dockmgr = RigelEditorApp.Instance.EditorGUI.DockManager;
            //dockmgr.Update(dockgroup);
            //dockmgr.LateUpdate();
        }
    }
}
