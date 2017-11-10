using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public interface IGUIDockContent
    {

    }


    public abstract class GUIDockBase: IGUIDockObj
    {
        public Vector4 m_size = new Vector4(0,0,100, 100);
        public Vector4? m_sizeNext = null;
        public Vector4 m_containerRect;
        public abstract void Draw(Vector4 rect);
        public GUIDockGroup m_parent;
    }

    public interface IGUIDockObj
    {
         void Draw(Vector4 rect);
    }

    public class GUIDockSeparator: IGUIDockObj
    {
        public GUIDockGroup m_parent;
        private bool m_ondrag = false;

        private Vector4 m_rect;
        private Vector4 m_rectab;
        public void Draw(Vector4 rect)
        {
            m_rect = rect;
            m_rectab = GUI.GetRectAbsolute(m_rect);
            if(m_parent.m_orient == GUIDockGroup.GUIDockOrient.Horizontal)
            {
                m_rectab.X -= 2;
                m_rectab.Z += 4;
            }
            else
            {
                m_rectab.Y -= 2;
                m_rectab.W += 4;
            }
            GUI.DrawRect(rect,GUIStyle.Current.DockSepColor);
            CheckDrag();
        }

        private void CheckDrag()
        {
            if (GUI.Event.Used) return;
            var e = GUI.Event;
            if(e.EventType == RigelEGUIEventType.MouseDragEnter)
            {
                if (GUIUtility.RectContainsCheck(m_rectab, e.Pointer)){
                    m_ondrag = true;
                    e.Use();
                    GUIInternal.SetCursor(m_parent.m_orient == GUIDockGroup.GUIDockOrient.Horizontal ? System.Windows.Forms.Cursors.VSplit : System.Windows.Forms.Cursors.HSplit);
                }
            }
            else if (e.EventType == RigelEGUIEventType.MouseDragLeave)
            {
                if (m_ondrag)
                {
                    e.Use();
                    m_ondrag = false;

                    GUIInternal.SetCursor(System.Windows.Forms.Cursors.Default);
                }
            }
            else if(e.EventType == RigelEGUIEventType.MouseDragUpdate)
            {
                if (m_ondrag)
                {
                    e.Use();
                    m_parent.SeparatorMove(e.DragOffset, this);
                }
            }

        }
    }

    public class GUIDock : GUIDockBase
    {
        public object m_target;
        public List<IGUIDockContent> m_content = new List<IGUIDockContent>();
        private IGUIDockContent m_focus = null;


        private Vector4 m_contentRect;

        public override void Draw(Vector4 rect)
        {
            m_size = rect;
            m_containerRect = m_size;
            m_containerRect = m_containerRect.Padding(1);

            GUI.BeginGroup(m_containerRect, GUIStyle.Current.DockBGColor);

            GUILayout.BeginArea(GUI.Context.currentGroupAbsolute);

            DrawTabBar();

            m_contentRect = GUI.Context.currentGroup;
            m_contentRect.X = 0;
            m_contentRect.Y = 23;
            m_contentRect.W -= 23;
            GUI.BeginGroup(m_contentRect);
            GUILayout.BeginArea(GUI.Context.currentGroupAbsolute,GUIStyle.Current.DockContentColor);
            GUI.Label(new Vector4(0, 0, 100, 20), "Group");
            GUILayout.Button("Test");
            GUILayout.Text("66666");

            GUILayout.EndArea();

            GUI.EndGroup();
            GUILayout.EndArea();
            GUI.EndGroup();
        }

        private void DrawTabBar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.SetLineHeight(20);
            GUILayout.Space(3);

            if (GUILayout.Button("+", GUIStyle.Current.TabBtnColor))
            {
                var win = new GUIWindowTest1();
                m_content.Add(win);

                m_focus = win;
            }

            if (GUILayout.Button("-", GUIStyle.Current.TabBtnColor))
            {
                if(m_focus != null)
                {
                    m_content.Remove(m_focus);
                    if (m_content.Count != 0) m_focus = m_content[0];
                }
            }

            if (m_content.Count == 0)
            {
                GUILayout.Text("None");
            }
            else
            {
                foreach(var c in m_content)
                {
                    if(m_focus == c)
                    {
                        GUILayout.Button("Tab", GUIStyle.Current.TabBtnColorActive);
                    }
                    else
                    {
                        if(GUILayout.Button("Tab", GUIStyle.Current.TabBtnColor))
                        {
                            m_focus = c;
                        }
                    }
                }
            }
            

            GUILayout.RestoreLineHeight();
            GUILayout.EndHorizontal();
        }
    }

    public class GUIDockGroup : GUIDockBase
    {
        private static float SizeMin = 30;

        public enum GUIDockOrient
        {
            Horizontal,
            Vertical
        };

        public List<IGUIDockObj> m_children = new List<IGUIDockObj>();
        public GUIDockOrient m_orient = GUIDockOrient.Horizontal;

        public GUIDockGroup()
        {

        }

        public override void Draw(Vector4 size)
        {
            m_size = size;
            GUI.BeginGroup(m_size);
            ReiszeChild();
            GUI.EndGroup();
        }

        private void ReiszeChild()
        {
            float stotal = 0;
            float stepSize = 0;
            foreach(var c in m_children)
            {
                if (c is GUIDockSeparator) continue;

                var dock = c as GUIDockBase;
                if(dock.m_sizeNext != null)
                {
                    dock.m_size = (Vector4)dock.m_sizeNext;
                    dock.m_sizeNext = null;
                }
                if(m_orient == GUIDockOrient.Horizontal)
                {
                    stotal += dock.m_size.Z;
                }
                else
                {
                    stotal += dock.m_size.W;
                }
            }

            Vector4 crect = Vector4.Zero;
            if(m_orient == GUIDockOrient.Horizontal)
            {
                crect.W = m_size.W;
                stepSize = m_size.Z / stotal;
            }
            else
            {
                crect.Z = m_size.Z;
                stepSize = m_size.W / stotal;
            }

            int count = 0;
            int total = m_children.Count;
            foreach(var c in m_children)
            {
                if(c is GUIDockBase)
                {
                    var dock = c as GUIDockBase;

                    if (m_orient == GUIDockOrient.Horizontal)
                    {
                        crect.Z = dock.m_size.Z * stepSize;
                        dock.Draw(crect);
                        crect.X += dock.m_size.Z;
                    }
                    else
                    {
                        crect.W = dock.m_size.W * stepSize;
                        dock.Draw(crect);
                        crect.Y += dock.m_size.W;
                    }
                }
                else
                {
                    //seperator
                    if (count < total - 1)
                    {
                        if (m_orient == GUIDockOrient.Horizontal)
                        {
                            crect.X -= 1;
                            crect.Z = 2;
                            c.Draw(crect);
                            crect.X += 1;
                        }
                        else
                        {
                            crect.Y -= 1;
                            crect.W = 2;
                            c.Draw(crect);
                            crect.Y += 1;
                        }
                    }
                }

                count++;
            }
        }

        public void AddDock(GUIDockBase dock)
        {
            if(m_children.Count != 0)
            {
                var sep = new GUIDockSeparator();
                sep.m_parent = this;
                m_children.Add(sep);
            }
            m_children.Add(dock);
            dock.m_parent = this;
        }

        public void SeparatorMove(Vector2 offset,GUIDockSeparator sep)
        {
            for(int i = 0; i < m_children.Count; i++)
            {
                if(m_children[i] == sep)
                {
                    var predock = m_children[i - 1] as GUIDockBase;
                    var nextdock = m_children[i + 1] as GUIDockBase;

                    var rect1 = predock.m_size;
                    var rect2 = nextdock.m_size;
                    if (m_orient == GUIDockOrient.Horizontal)
                    {
                        float off = offset.X;
                        if(rect1.Z + off < GUIDockGroup.SizeMin)
                        {
                            off = GUIDockGroup.SizeMin - rect1.Z;
                        }
                        if (rect2.Z - off < GUIDockGroup.SizeMin)
                        {
                            off = rect2.Z - GUIDockGroup.SizeMin;
                        }

                        rect1.Z += off;
                        rect2.Z -= off;

                        predock.m_sizeNext = rect1;
                        nextdock.m_sizeNext = rect2;
                    }
                    else
                    {
                        float off = offset.Y;
                        if (rect1.W + off < GUIDockGroup.SizeMin)
                        {
                            off = GUIDockGroup.SizeMin - rect1.W;
                        }
                        if (rect2.W - off < GUIDockGroup.SizeMin)
                        {
                            off = rect2.W - GUIDockGroup.SizeMin;
                        }

                        rect1.W += off;
                        rect2.W -= off;

                        predock.m_sizeNext = rect1;
                        nextdock.m_sizeNext = rect2;
                    }
                }
            }
        }

    }



    public class GUIDockManager
    {
        private GUIDockGroup m_maingroup;

        public GUIDockManager()
        {
            m_maingroup = new GUIDockGroup();

            var group1 = new GUIDockGroup();
            group1.m_orient = GUIDockGroup.GUIDockOrient.Vertical;
            group1.AddDock(new GUIDock());
            group1.AddDock(new GUIDock());
            group1.AddDock(new GUIDock());
            m_maingroup.AddDock(new GUIDock());
            m_maingroup.AddDock(group1);
        }

        public void Update(Vector4 group)
        {
            GUI.BeginGroup(group,GUIStyle.Current.BorderColor);
            group.X = 0;
            group.Y = 0;
            m_maingroup.Draw(group);

            GUI.EndGroup();
        }
    }
}
