using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{

    public enum GUIDockOrient
    {
        Horizontal,
        Vertical
    };


    public enum GUIDockPlace
    {
        none,
        center,
        left,
        right,
        top,
        bottom,
    }

    public class GUIDockInfo
    {
        public Vector4 m_size = new Vector4(0, 0, 100, 100);
        public Vector4? m_sizeNext = null;
        public Vector4 m_containerRect;
        public GUIDockOrient m_orient = GUIDockOrient.Horizontal;


        public Vector4 m_sizeab;
        public Vector4 m_contentRect;
    }

    public class GUIDockNode
    {
        public GUIDockMgr m_mgr;

        public GUIDockNode m_nodeL;
        public GUIDockNode m_nodeR;
        public GUIDockNode m_parent;

        public List<GUIDockContentBase> m_content;
        public GUIDockContentBase m_contentFocus = null;
        public GUIDockSeparator m_separator;
        public GUIDockInfo m_info = new GUIDockInfo();


        public GUIDockNode()
        {
            m_content = new List<GUIDockContentBase>();
        }

        public GUIDockNode(GUIDockNode parent, GUIDockContentBase c)
        {
            m_parent = parent;
            m_content = new List<GUIDockContentBase>();
            m_content.Add(c);
            m_contentFocus = c;
        }

        public GUIDockNode(GUIDockNode parent,List<GUIDockContentBase> cs)
        {
            m_parent = parent;
            m_content = cs;
            if(m_contentFocus == null || !cs.Contains(m_contentFocus))
            {
                if(cs.Count > 0)
                {
                    m_contentFocus = m_content[0];
                }
                else
                {
                    m_contentFocus = null;
                }
            }
        }

        public void AddContent(GUIDockContentBase c)
        {
            if (m_nodeL != null || m_nodeR != null) throw new Exception();

            if (m_content == null) m_content = new List<GUIDockContentBase>();
            m_content.Add(c);
            m_contentFocus = c;
        }

        public void AddContent(GUIDockContentBase c,GUIDockPlace place)
        {
            if(place == GUIDockPlace.center)
            {
                AddContent(c);
                return;
            }else if(place != GUIDockPlace.none)
            {
                bool p = (place == GUIDockPlace.left || place == GUIDockPlace.top);
                var contents = m_content;
                var node1 = new GUIDockNode(this, c);
                var node2 = new GUIDockNode(this, contents);

                node1.m_contentFocus = c;
                node2.m_contentFocus = m_contentFocus;

                m_nodeL = p ? node1 : node2;
                m_nodeR = p ? node2 : node1;

                m_content = null;
                m_contentFocus = null;

                m_info.m_orient = (place == GUIDockPlace.left || place == GUIDockPlace.right) ? GUIDockOrient.Horizontal : GUIDockOrient.Vertical;
            }
        }

        public void RemoveContent(GUIDockContentBase c)
        {
            if (m_content == null) return;
            for (int i = 0; i < m_content.Count; i++)
            {
                if (m_content[i] == c)
                {
                    m_content.RemoveAt(i);
                    if(m_contentFocus == c)
                    {
                        if (m_content.Count != 0)
                        {
                            m_contentFocus = m_content[0];
                        }
                        else
                        {
                            m_contentFocus = null;
                        }
                    }
                    return;
                }
            }
        }
        public bool IsContentNode()
        {
            return (m_nodeL == null && m_nodeR == null);
        }

        public void Update(Vector4 size)
        {
            m_info.m_size = size;
            GUI.BeginGroup(size);

            //draw child
            if (IsContentNode())
            {
                DrawDock();
            }
            else
            {
                bool horizontal = m_info.m_orient == GUIDockOrient.Horizontal;

                float sl = horizontal ? m_nodeL.m_info.m_size.Z : m_nodeL.m_info.m_size.W;
                float sr = horizontal ? m_nodeR.m_info.m_size.Z : m_nodeR.m_info.m_size.W;

                float stepSize =0;

                Vector4 crect = Vector4.Zero;
                if (horizontal)
                {
                    crect.W = m_info.m_size.W;
                    stepSize = m_info.m_size.Z / (sl + sr);
                }
                else
                {
                    crect.Z = m_info.m_size.Z;
                    stepSize = m_info.m_size.W / (sl + sr);
                }

                if (horizontal)
                {
                    //nodeleft
                    crect.Z = sl * stepSize;
                    m_nodeL.Update(crect);
                    crect.X += crect.Z;

                    //noderight
                    crect.Z = sr * stepSize;
                    m_nodeR.Update(crect);
                    crect.X += crect.Z;
                }
                else
                {
                    //nodeleft
                    crect.W = sl * stepSize;
                    m_nodeL.Update(crect);
                    crect.Y += crect.W;

                    //noderight
                    crect.W = sr * stepSize;
                    m_nodeR.Update(crect);
                    crect.Y += crect.W;
                }
            }
            GUI.EndGroup();
        }

        public void LateUpdate()
        {

        }

        public GUIDockNode GetRoot()
        {
            if (m_parent == null) return this;
            var p = m_parent;

            int count = 0;
            while(p.m_parent != null)
            {
                p = p.m_parent;
                count++;
            }
            return p;
        }

        public GUIDockNode CheckDockNodeMatched()
        {

            if (IsContentNode())
            {
                if (GUIUtility.RectContainsCheck(m_info.m_sizeab, GUI.Event.Pointer))
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var match = m_nodeL.CheckDockNodeMatched();
                

                if(match == null)
                {
                    match = m_nodeR.CheckDockNodeMatched();
                }
                return match;
            }
        }

        private bool CheckPlaceValid(GUIDockPlace place,GUIDockNode src)
        {
            if(this == src)
            {
                if (src.m_content.Count == 1) return false;
                if (place == GUIDockPlace.center) return false;
                return true;
            }
            else
            {
                return true;
            }
        }

        public void UpdateNode()
        {
            if (IsContentNode())
            {
                if(m_content.Count == 0)
                {
                    if(m_parent != null)
                    {
                    }
                }
            }
        }

        public void CollapseNode(bool left)
        {
        }

        #region Draw
        private void DrawDock()
        {
            m_info.m_containerRect = m_info.m_size.Padding(1);
            m_info.m_containerRect.X = 0;
            m_info.m_containerRect.Y = 0;

            GUI.BeginGroup(m_info.m_containerRect, GUIStyle.Current.DockBGColor);
            {
                m_info.m_sizeab = GUI.Context.currentGroup.Absolute;
                GUILayout.BeginArea(GUI.Context.currentGroup.Absolute);
                {
                    DrawTabBar();

                    var contentRect = GUI.Context.currentGroup.Rect;
                    contentRect.X = 0;
                    contentRect.Y = 23;
                    contentRect.W -= 23;
                    m_info.m_contentRect = contentRect;

                    GUI.BeginGroup(m_info.m_contentRect);
                    {
                        GUILayout.BeginArea(GUI.Context.currentGroup.Absolute, GUIStyle.Current.DockContentColor);
                        {
                            //test draw
                            GUILayout.Button("TestDraw");
                        }
                        GUILayout.EndArea();
                    }
                    GUI.EndGroup();
                }
                GUILayout.EndArea();
            }
            GUI.EndGroup();
        }

        private void DrawTabBar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.SetLineHeight(20);
                {
                    if (GUILayout.Button("+", GUIStyle.Current.TabBtnColorS, GUIOption.Width(20)))
                    {
                        var win = new GUIWindowTest1();
                        AddContent(win);

                    }
                    if (GUILayout.Button("-", GUIStyle.Current.TabBtnColorS, GUIOption.Width(20)))
                    {
                        if (m_info != null)
                        {
                            RemoveContent(m_contentFocus);
                        }
                    }
                    if(m_content.Count == 0)
                    {
                        GUILayout.Text("No-Content");
                    }
                    else
                    {
                        foreach (var c in m_content)
                        {
                            GUILayout.Space(3);
                            DrawTabBarContentBtn(c);
                            GUILayout.Space(-3);
                        }
                    }
                }
                GUILayout.RestoreLineHeight();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawTabBarContentBtn(GUIDockContentBase c)
        {
            var checkrc = GUIOption.Check(GUIOptionCheck.rectContains);
            if (GUILayout.Button(c.Title, m_contentFocus == c ? GUIStyle.Current.TabBtnColorActive : GUIStyle.Current.TabBtnColor, checkrc))
            {
                m_contentFocus = c;
            }

            if (c.InternalTabBtnDragState.OnDrag(checkrc.Checked()))
            {

                var root = GetRoot();

                var matchedDock = root.CheckDockNodeMatched();
                if (matchedDock == null)
                {

                }
                else
                {
                    var dockplace = matchedDock.DrawOnDockContentDrag(c,this);
                    if(dockplace != GUIDockPlace.none)
                    {
                        Console.WriteLine(">> " + dockplace);

                        GetRoot().m_mgr.SetDockPlace(dockplace, c, this, matchedDock);
                    }
                }
            }
        }

        private GUIDockPlace DrawOnDockContentDrag(GUIDockContentBase content,GUIDockNode src)
        {
            GUIDockPlace dockPlace = GUIDockPlace.none;
            GUI.BeginGroup(m_info.m_sizeab, null, true);
            {
                GUI.BeginDepthLayer(1);
                {
                    float rsize = 40;
                    var center = m_info.m_sizeab.Size() * 0.5f;
                    var rectbasic = new Vector4(center - rsize * 0.5f * Vector2.One, rsize, rsize);
                    bool activeChecked = false;

                    var pointer = GUI.Event.Pointer;
                    
                    //center
                    if(CheckPlaceValid(GUIDockPlace.center,src))
                    {
                        if (!activeChecked && GUIUtility.RectContainsCheck(GUI.GetRectAbsolute(rectbasic), pointer))
                        {
                            GUI.DrawRect(rectbasic, GUIStyle.Current.ColorActive);
                            activeChecked = true;
                            dockPlace = GUIDockPlace.center;
                        }
                        else
                        {
                            GUI.DrawRect(rectbasic, GUIStyle.Current.ColorActiveD);
                        }
                    }
                    else
                    {
                        GUI.DrawRect(rectbasic, GUIStyle.Current.ColorDisabled);
                    }

                    //left
                    var rect = rectbasic.Move(-35, 0).SetSize(30, rsize);
                    if (CheckPlaceValid(GUIDockPlace.left, src))
                    {
                        if (!activeChecked && GUIUtility.RectContainsCheck(GUI.GetRectAbsolute(rect), pointer))
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActive);
                            activeChecked = true;
                            dockPlace = GUIDockPlace.left;
                        }
                        else
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActiveD);
                        }
                    }
                    else
                    {
                        GUI.DrawRect(rect, GUIStyle.Current.ColorDisabled);
                    }

                    //right
                    rect = rectbasic.Move(45, 0).SetSize(30, rsize);
                    if (CheckPlaceValid(GUIDockPlace.right, src))
                    {
                        if (!activeChecked && GUIUtility.RectContainsCheck(GUI.GetRectAbsolute(rect), pointer))
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActive);
                            activeChecked = true;
                            dockPlace = GUIDockPlace.right;
                        }
                        else
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActiveD);
                        }
                    }
                    else
                    {
                        GUI.DrawRect(rect, GUIStyle.Current.ColorDisabled);
                    }

                    //top
                    rect = rectbasic.Move(0, -35).SetSize(rsize, 30);
                    if (CheckPlaceValid(GUIDockPlace.top, src))
                    {
                        if (!activeChecked && GUIUtility.RectContainsCheck(GUI.GetRectAbsolute(rect), pointer))
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActive);
                            activeChecked = true;
                            dockPlace = GUIDockPlace.top;
                        }
                        else
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActiveD);
                        }
                    }
                    else
                    {
                        GUI.DrawRect(rect, GUIStyle.Current.ColorDisabled);
                    }

                    //bottom
                    rect = rectbasic.Move(0, 45).SetSize(rsize, 30);
                    if (CheckPlaceValid(GUIDockPlace.bottom, src))
                    {
                        if (!activeChecked && GUIUtility.RectContainsCheck(GUI.GetRectAbsolute(rect), pointer))
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActive);
                            activeChecked = true;
                            dockPlace = GUIDockPlace.bottom;
                        }
                        else
                        {
                            GUI.DrawRect(rect, GUIStyle.Current.ColorActiveD);
                        }
                    }
                    else
                    {
                        GUI.DrawRect(rect, GUIStyle.Current.ColorDisabled);
                    }
                }
                GUI.EndDepthLayer();
            }
            GUI.EndGroup();

            if (content.InternalTabBtnDragState.Stage != GUIDrawStateStage.Exit)
            {
                return GUIDockPlace.none;
            }

            return dockPlace;
        }

        #endregion
    }

    public class GUIDockMgr
    {
        private GUIDockNode m_root;
        private EventSlot<Action> m_slotDockPlace = new EventSlot<Action>();

        public GUIDockMgr()
        {
            m_root = new GUIDockNode();
            m_root.m_mgr = this;
        }

        public void Update(Vector4 group)
        {
            GUI.BeginGroup(group, GUIStyle.Current.BorderColor);
            group.X = 0;
            group.Y = 0;
            m_root.Update(group);
            GUI.EndGroup();
        }

        public void LateUpdate()
        {
            m_slotDockPlace.InvokeOnce();
        }

        public void SetDockPlace(GUIDockPlace place,GUIDockContentBase content,GUIDockNode src,GUIDockNode dst)
        {
            m_slotDockPlace += () => { SetDockPlaceImpl(place, content, src, dst); };
        }
        private void SetDockPlaceImpl(GUIDockPlace place, GUIDockContentBase content, GUIDockNode src, GUIDockNode dst)
        {
            //remove
            src.RemoveContent(content);

            //append
            dst.AddContent(content, place);

            //refresh
            src.UpdateNode();
            dst.UpdateNode();
        }

    }

}
