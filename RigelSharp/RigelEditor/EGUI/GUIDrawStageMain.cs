using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    class GUIDrawStageMain : GUIDrawStage
    {
        private bool m_initedDraw = false;
        private bool m_needUpdateBuffer = false;

        private int m_lastBufferSizeRect = 0;
        private int m_lastBufferSizeText = 0;


        private GUIMenuList m_menuList;

        //dock
        private GUIDockMgr m_dockMgr;
        private Vector4 m_dockGroup;

        

        public GUIDrawStageMain(string stagename, int order = 0) : base(stagename, order)
        {
            m_menuList = new GUIMenuList("File");
            m_menuList.AddMenuItem("File/Test");
            m_menuList.AddMenuItem("File/Test2");
            m_menuList.AddMenuItem("Open/Sln");
            m_menuList.AddMenuItem("Exit");

            m_dockMgr = new GUIDockMgr();
        }

        public override void Draw(RigelEGUIEvent guievent)
        {
            base.Draw(guievent);

            m_needUpdateBuffer = false;
            if (guievent.Used && m_initedDraw) return; 

            m_drawTarget.bufferRect.Clear();
            m_drawTarget.bufferText.Clear();


            DrawContent();

            m_needUpdateBuffer = true;
        }

        public override void SyncBuffer(RigelEGUICtx eguictx)
        {
            if (!m_needUpdateBuffer) return;

            var bind = eguictx.GraphicsBind;
            {
                var rectCount = m_drawTarget.bufferRect.Count;
                if (rectCount != m_lastBufferSizeRect) bind.NeedRebuildCommandList = true;
                bind.BufferMainRect.CheckAndExtendsWithSize(rectCount);
                m_drawTarget.bufferRect.CopyTo(bind.BufferMainRect.BufferData);
                bind.BufferMainRect.InternalSetBufferDataCount(rectCount);

                m_lastBufferSizeRect = rectCount;
            }

            {
                var textCount = m_drawTarget.bufferText.Count;
                if (textCount != m_lastBufferSizeText) bind.NeedRebuildCommandList = true;
                bind.BufferMainText.CheckAndExtendsWithSize(textCount);
                m_drawTarget.bufferText.CopyTo(bind.BufferMainText.BufferData);
                bind.BufferMainText.InternalSetBufferDataCount(textCount);

                m_lastBufferSizeText = textCount;
            }
        }




        private void DrawContent()
        {
            GUILayout.BeginToolBar(23);
            //GUILayout.DrawMenuList(m_menuList);
            GUILayout.Button("bbb",GUIOption.Width(50));
            GUILayout.Text("HelloWorld");
            GUILayout.Text("ABCDEEEDASDWDS",GUIOption.Width(50));
            GUILayout.Button("TestBtn");
            GUILayout.EndToolBar();

            m_dockGroup = GUI.Context.currentGroup.Rect;
            m_dockGroup.Y = 23;
            m_dockGroup.W -= 23;

            m_dockMgr.Update(m_dockGroup);
            m_dockMgr.LateUpdate();
        }
    }
}
