using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{

    public abstract class GUIDockBase: IGUIDockObj
    {
        public Vector4 m_size = new Vector4(0,0,100, 100);
        public Vector4 m_contentRect;
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

        private Vector4 m_rect;
        public void Draw(Vector4 rect)
        {
            m_rect = rect;
            GUI.DrawRect(rect, RigelColor.Red);

            CheckDrag();
        }

        private void CheckDrag()
        {
            if (GUI.Event.Used) return;
            var e = GUI.Event;

        }
    }

    public class GUIDock : GUIDockBase
    {
        public object m_target;
        

        private Vector4 m_color = RigelColor.Random();

        public override void Draw(Vector4 rect)
        {
            m_size = rect;
            m_contentRect = m_size;
            m_contentRect = m_contentRect.Padding(1);

            GUI.BeginGroup(m_contentRect, GUIStyle.Current.DockBGColor);
            GUILayout.BeginArea(GUI.Context.currentGroupAbsolute);

            GUI.Label(new Vector4(0, 0, 100, 20), "Group");

            GUILayout.Space(20);
            GUILayout.Button("Test");


            GUILayout.EndArea();
            GUI.EndGroup();
        }
    }

    public class GUIDockGroup : GUIDockBase
    {
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
            m_maingroup.AddDock(new GUIDock());
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
