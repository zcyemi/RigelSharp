using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.EGUI
{
    public class GUIDrawStageOverlay : GUIDrawStage
    {

        private bool m_needSyncBuffer = false;


        private int m_lastBufferSizeRect = 0;
        private int m_lastBufferSizeText = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stagename"></param>
        /// <param name="order"> near(0) ------ far(500) </param>
        public GUIDrawStageOverlay(string stagename, int order = 0): base(stagename, order)
        {

        }

        public override void Draw(GUIEvent guievent)
        {
            base.Draw(guievent);

            //do draw
            m_needSyncBuffer = false;
            var comp = GUI.Context.componentStack;
            if (comp.Count == 0)
            {
                return;
            }


            //every draw the last one
            IGUIComponent curcomp = comp.Peek();
            if (curcomp.InitDrawed)
            {
                m_drawTarget.bufferRect.RemoveRange(curcomp.BufferRectStart,curcomp.BufferRectCount);
                m_drawTarget.bufferText.RemoveRange(curcomp.BufferTextStart, curcomp.BufferTextCount);
            }

            int rectCount = m_drawTarget.bufferRect.Count;
            int textCount = m_drawTarget.bufferText.Count;
            curcomp.BufferRectStart = rectCount;
            curcomp.BufferTextStart = textCount;

            //get relative depth;
            if(comp.Count > 1)
            {
                float lastDepth = GUI.Depth;
                if(m_drawTarget.bufferRect.Count > 0)
                {
                    lastDepth = Math.Min(lastDepth,m_drawTarget.bufferRect.VerticesZ(rectCount - 1));
                }
                if(m_drawTarget.bufferText.Count > 0)
                {
                    lastDepth = Math.Min(lastDepth, m_drawTarget.bufferText.VerticesZ(textCount - 1));
                }
                GUI.SetDepth(lastDepth);
            }

            curcomp.Draw(guievent);
            guievent.Use();

            curcomp.BufferRectEnd = m_drawTarget.bufferRect.Count;
            curcomp.BufferTextEnd = m_drawTarget.bufferText.Count;

            curcomp.InitDrawed = true;
            m_needSyncBuffer = true;
        }

        public override void SyncBuffer(IGUIGraphicsBind guibind)
        {
            var bind = guibind;

            if (!m_needSyncBuffer)
            {
                if (GUI.Context.componentStack.Count == 0 && (m_lastBufferSizeText != 0 || m_lastBufferSizeRect != 0))
                {
                    m_lastBufferSizeText = 0;
                    m_lastBufferSizeRect = 0;
                    bind.NeedRebuildCommandList = true;

                }
                return;
            }

            bind.SyncDrawTarget(this,m_drawTarget);

            //remove bufferdata
            var compstack = GUI.Context.componentStack;
            while (true)
            {
                if (compstack.Count == 0) break;
                var comp = compstack.Peek();
                if (comp.Destroy)
                {
                    m_drawTarget.bufferRect.RemoveRange(comp.BufferRectStart, comp.BufferRectCount);
                    m_drawTarget.bufferText.RemoveRange(comp.BufferTextStart, comp.BufferTextCount);
                    compstack.Pop();

                }
                else
                {
                    break;
                }
            }


        }
    }
}
