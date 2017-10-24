using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RigelEditor.EGUI
{
    public partial class RigelEGUICtx
    {
        private RigelEGUIDockerManager m_dockerMgr;
        private RigelEGUIMenu m_mainMenu;
        private List<RigelEGUIWindow> m_windows;


        private List<RigelEGUIVertex> m_bufferRect = new List<RigelEGUIVertex>();
        private List<RigelEGUIVertex> m_bufferText = new List<RigelEGUIVertex>();

        internal List<RigelEGUIVertex> BufferRect { get { return m_bufferRect; } }
        internal List<RigelEGUIVertex> BufferText { get { return m_bufferText; } }

        private Dictionary<int, int> m_bufferRectEmptyBlock = new Dictionary<int, int>(); //begin end

        private void GUIInit()
        {
            m_windows = new List<RigelEGUIWindow>();

            m_mainMenu = new RigelEGUIMenu();
            RefreshMainMenu();

            m_dockerMgr = new RigelEGUIDockerManager();

            RigelEGUI.s_currentCtx = this;

        }

        private void GUIRelease()
        {
            m_windows.Clear();
            m_mainMenu.ClearAllItems();
        }


        private void GUIUpdate(RigelEGUIEvent guievent)
        {
            m_bufferRectEmptyBlock.Clear();

            //make sure the focused window at the first
            m_windows.Sort((a, b) => { return b.Focused.CompareTo(a.Focused); });

            //draw menu

            int focusedWin = 0;

            int totalBufferSizeCount = 0;
            foreach (var win in m_windows)
            {
                UpdateWindow(win,guievent);
                if (win.Focused) focusedWin++;

                totalBufferSizeCount += win.m_bufferInfo.BufferRectEndPos - win.m_bufferInfo.BufferRectStartPos;
            }

            RigelUtility.Assert(focusedWin <= 1, "[Exception] Multi Focused Window :"+focusedWin);

            // reset window order
            // Buffer Struct: Lesser  <<------ window.Order ------ << Greater
            // the num of array copy operation is minium when focused window at the end of buffer
            // it doesn't influence the effecient of rendering pipeline
            m_windows.Sort((a,b)=>{ return a.Order.CompareTo(b.Order); });

            //arrange buffer
            {
                var newbuf = new RigelEGUIVertex[m_graphicsBind.BufferVertexRect.BufferSize];
                var bufrect = m_graphicsBind.BufferVertexRect.BufferData;
                int newbufPos = 0;
                foreach(var win in m_windows)
                {
                    int winbufsize = win.m_bufferInfo.BufRectSize;
                    Array.Copy(
                        bufrect, 
                        win.m_bufferInfo.BufferRectStartPos, 
                        newbuf, 
                        newbufPos,
                        winbufsize);
                    win.m_bufferInfo.BufferRectStartPos = newbufPos;
                    newbufPos += winbufsize;
                    win.m_bufferInfo.BufferRectEndPos = newbufPos;
                }
                m_graphicsBind.BufferVertexRect.BufferData = newbuf;
                m_graphicsBind.BufferVertexRect.InternalSetBufferDataCount(totalBufferSizeCount);
            }

        }

        private void UpdateWindow(RigelEGUIWindow win,RigelEGUIEvent guievent)
        {
            bool needupdate = false;
            if (win.m_bufferInfo.BufferInited == false) needupdate = true;

            if (((int)guievent.EventType | (int)RigelEGUIEventType.MouseEventActive) > 0)
            {
                if(guievent.InternalFocusedWindow != null || guievent.Used)
                {
                    win.Focused = false;
                }
                else
                {
                    if (RigelEGUI.RectContainsCheck(win.Position, win.Size, guievent.Pointer))
                    {
                        RigelUtility.Log("WindowFocused:" + win.ToString());
                        guievent.InternalFocusedWindow = win;
                        win.Focused = true;
                        win.m_order = GetMaxWindowOrder() + 1;
                    }
                    else
                    {
                        win.Focused = false;
                    }
                }
            }
            else
            {
                //passive focus window
                if (win.Focused)
                {
                    guievent.InternalFocusedWindow = win;
                }
            }

            needupdate |= win.Focused;

            if (needupdate)
            {
                InternalBeginWindow(win);
                win.InternalDrawBasis();
                win.OnGUI();
                InternalEndWindow();
            }

        }


        private void InternalBeginWindow(RigelEGUIWindow win)
        {
            m_bufferRect.Clear();
            m_bufferText.Clear();
            RigelEGUI.s_currentWindow = win;
        }

        private void InternalEndWindow()
        {
            var curwin = RigelEGUI.s_currentWindow;
            RigelEGUI.s_currentWindow = null;

            var buffercount = m_bufferRect.Count();

            if (buffercount == 0)
            {
                RigelUtility.Log("temp draw buffer count:" + buffercount);
                return;
            }
            //applytobuffer
            if (curwin.m_bufferInfo.BufferInited == false)
            {
                curwin.m_bufferInfo.BufferRectStartPos = m_graphicsBind.BufferVertexRect.BufferDataCount;


                Array.Copy(
                    m_bufferRect.ToArray(),
                    0,
                    m_graphicsBind.BufferVertexRect.BufferData,
                    m_graphicsBind.BufferVertexRect.BufferDataCount,
                    buffercount
                );

                m_graphicsBind.BufferVertexRect.IncreaseBufferDataCount(buffercount);
                curwin.m_bufferInfo.BufferRectEndPos = m_graphicsBind.BufferVertexRect.BufferDataCount;

                curwin.m_bufferInfo.BufferInited = true;

                RigelUtility.Log("buffer extends:" + buffercount);
            }
            else
            {
                if (curwin.m_bufferInfo.BufferRectEndPos == m_graphicsBind.BufferVertexRect.BufferDataCount)
                {
                    //buffer at end
                    Array.Copy(
                        m_bufferRect.ToArray(),
                        0,
                        m_graphicsBind.BufferVertexRect.BufferData,
                        curwin.m_bufferInfo.BufferRectStartPos,
                        buffercount
                    );
                    m_graphicsBind.BufferVertexRect.InternalSetBufferDataCount(curwin.m_bufferInfo.BufferRectStartPos + buffercount);
                    curwin.m_bufferInfo.BufferRectEndPos = m_graphicsBind.BufferVertexRect.BufferDataCount;
                }
                else
                {
                    //buffer not at end
                    if (curwin.m_bufferInfo.BufferRectEndPos - curwin.m_bufferInfo.BufferRectStartPos != 0)
                    {
                        m_bufferRectEmptyBlock.Add(curwin.m_bufferInfo.BufferRectStartPos, curwin.m_bufferInfo.BufferRectEndPos);
                    }
                    else
                    {
                        RigelUtility.Log("window Buffer size is zero");
                    }

                    curwin.m_bufferInfo.BufferRectStartPos = m_graphicsBind.BufferVertexRect.BufferDataCount;
                    Array.Copy(
                        m_bufferRect.ToArray(),
                        0,
                        m_graphicsBind.BufferVertexRect.BufferData,
                        m_graphicsBind.BufferVertexRect.BufferDataCount,
                        buffercount
                        );
                    m_graphicsBind.BufferVertexRect.IncreaseBufferDataCount(buffercount);
                    curwin.m_bufferInfo.BufferRectEndPos = m_graphicsBind.BufferVertexRect.BufferDataCount;
                }
            }

            RigelUtility.Assert(curwin.m_bufferInfo.BufferRectEndPos >= curwin.m_bufferInfo.BufferRectStartPos);

            m_graphicsBind.NeedRebuildCommandList = true;
        }


        private void RefreshMainMenu()
        {
            var assembly = RigelReflectionHelper.AssemblyRigelEditor;
            foreach (var type in assembly.GetTypes())
            {
                var methods = RigelReflectionHelper.GetMethodByAttribute<RigelEGUIMenuItemAttribute>(
                    type,
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

                foreach (var m in methods)
                {
                    var attr = Attribute.GetCustomAttribute(m, typeof(RigelEGUIMenuItemAttribute)) as RigelEGUIMenuItemAttribute;
                    m_mainMenu.AddMenuItem(attr.Label, m);
                }

            }
            RigelUtility.Log("EGUI mainMenu item count:" + m_mainMenu.ItemNodes.Count());

        }

        private int GetMaxWindowOrder()
        {
            int order = 0;
            m_windows.ForEach((w) => { order = w.Order > order ? w.Order : order; });
            return order;
        }

    }
}
