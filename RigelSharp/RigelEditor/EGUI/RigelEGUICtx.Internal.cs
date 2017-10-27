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

        private bool m_bufferRectEmptyBlock = false;
        private bool m_bufferTextEmptyBlock = false;

        private void GUIInit()
        {
            ClientWidth = m_form.Width;
            ClientHeight = m_form.Height;

            m_windows = new List<RigelEGUIWindow>();

            m_mainMenu = new RigelEGUIMenu();
            RefreshMainMenu();

            m_dockerMgr = new RigelEGUIDockerManager();

            RigelEGUI.InternalResetContext(this);

        }

        private void GUIRelease()
        {
            m_windows.Clear();
            m_mainMenu.ClearAllItems();
        }


        private void GUIUpdate(RigelEGUIEvent guievent)
        {
            RigelUtility.Log("------------- New Frame -----------");

            RigelEGUI.InternalFrameBegin(guievent);

            m_bufferRectEmptyBlock = false;
            m_bufferTextEmptyBlock = false;

            //make sure the focused window at the first
            m_windows.Sort((a, b) => { return b.Focused.CompareTo(a.Focused); });

            //draw menu

            int focusedWin = 0;

            int totalBufferRectSizeCount = 0;
            int totalBufferTextSizeCount = 0;
            foreach (var win in m_windows)
            {
                UpdateWindow(win,guievent);
                if (win.Focused) focusedWin++;

                totalBufferRectSizeCount += win.BufferInfoRect.Size;
                totalBufferTextSizeCount += win.BufferInfoText.Size;
            }

            RigelUtility.Assert(focusedWin <= 1, "[Exception] Multi Focused Window :"+focusedWin);

            // reset window order
            // Buffer Struct: Lesser  <<------ window.Order ------ << Greater
            // the num of array copy operation is minium when focused window at the end of buffer
            // it doesn't influence the effecient of graphics rendering pipeline
            if(m_windows.Count > 1)
            {
                m_windows.Sort((a, b) => { return a.Order.CompareTo(b.Order); });

                //need to shrink the window order to z-near/z-far range
                if(m_windows[m_windows.Count-1].Order >= RigelEGUIGraphicsBind.GUI_CLIP_PLANE_FAR)
                {
                    //TODO
                    //adjust all RigelEGUIVertex.Pos.z
                }
            }


            //draw end
            RigelEGUI.InternalFrameEnd();

            //arrange buffer
            if (m_bufferRectEmptyBlock)
            {
                var newbufRect = new RigelEGUIVertex[m_graphicsBind.BufferVertexRect.BufferSize];
                var bufRect = m_graphicsBind.BufferVertexRect.BufferData;
                int newbufPosRect = 0;
                foreach(var win in m_windows)
                {
                    //rect
                    int winbufsizeRect = win.BufferInfoRect.Size;
                    Array.Copy(
                        bufRect, 
                        win.BufferInfoRect.StartPos, 
                        newbufRect, 
                        newbufPosRect,
                        winbufsizeRect);
                    win.BufferInfoRect.StartPos = newbufPosRect;
                    newbufPosRect += winbufsizeRect;
                    win.BufferInfoRect.EndPos = newbufPosRect;
                }

                m_graphicsBind.BufferVertexRect.BufferData = newbufRect;
                m_graphicsBind.BufferVertexRect.InternalSetBufferDataCount(totalBufferRectSizeCount);

            }

            if (m_bufferTextEmptyBlock)
            {
                var newbufText = new RigelEGUIVertex[m_graphicsBind.BufferVertexText.BufferSize];
                var bufText = m_graphicsBind.BufferVertexText.BufferData;
                var newbufPosText = 0;
                foreach (var win in m_windows)
                {
                    //text
                    int winbufsizeText = win.BufferInfoText.Size;
                    Array.Copy(
                        bufText,
                        win.BufferInfoText.StartPos,
                        newbufText,
                        newbufPosText,
                        winbufsizeText);
                    win.BufferInfoText.StartPos = newbufPosText;
                    newbufPosText += winbufsizeText;
                    win.BufferInfoText.EndPos = newbufPosText;

                }
                m_graphicsBind.BufferVertexText.BufferData = newbufText;
                m_graphicsBind.BufferVertexText.InternalSetBufferDataCount(totalBufferTextSizeCount);

            }

        }

        private void UpdateWindow(RigelEGUIWindow win,RigelEGUIEvent guievent)
        {
            bool needupdate = false;
            bool lastFrameFocused = win.Focused;
            if (win.BufferInfoRect.Inited == false && win.BufferInfoText.Inited == false)
                needupdate = true;


            if ((guievent.EventType & RigelEGUIEventType.MouseEventActive) > 0)
            {
                if(guievent.InternalFocusedWindow != null || guievent.Used)
                {
                    win.Focused = false;
                }
                else
                {
                    if (RigelEGUI.RectContainsCheck(win.Position, win.Size, guievent.Pointer))
                    {
                        //RigelUtility.Log("WindowFocused:" + win.ToString());
                        guievent.InternalFocusedWindow = win;
                        if(win.Focused == false)
                        {
                            win.Focused = true;
                            win.m_order = GetMaxWindowOrder() + 1;
                        }
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

            //updateWindow when focused mode changed
            if (lastFrameFocused && !win.Focused) needupdate = true;

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
            RigelEGUI.InternalWindowBegin(win);
            RigelEGUI.s_depthz = RigelEGUIGraphicsBind.GUI_CLIP_PLANE_FAR - (win.Order + RigelEGUI.s_depthStep);
        }

        private void InternalEndWindow()
        {
            var curwin = RigelEGUI.CurrentWindow;
            RigelEGUI.InternalWindowEnd();


            //applytobuffer
            m_bufferRectEmptyBlock |= m_graphicsBind.BufferVertexRect.UpdateGUIWindowBuffer(ref curwin.BufferInfoRect, BufferRect);
            m_bufferTextEmptyBlock |= m_graphicsBind.BufferVertexText.UpdateGUIWindowBuffer(ref curwin.BufferInfoText, BufferText);

            RigelUtility.Assert(curwin.BufferInfoRect.EndPos >= curwin.BufferInfoRect.StartPos);
            RigelUtility.Assert(curwin.BufferInfoText.EndPos >= curwin.BufferInfoText.StartPos);
            

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
