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

        public GUIDrawStageMain(string stagename, int order = 0) : base(stagename, order)
        {
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


        private static GUIMenuList m_menuList = new GUIMenuList("File");

        private void DrawContent()
        {
            GUILayout.BeginToolBar(20);
            GUILayout.DrawMenuList(m_menuList);
            GUILayout.Button("bbb");
            GUILayout.Text("HelloWorld");
            GUILayout.Button("TestBtn");

            GUILayout.EndToolBar();
        }
    }
}
