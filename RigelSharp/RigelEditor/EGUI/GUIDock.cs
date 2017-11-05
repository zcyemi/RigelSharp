using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{

    public abstract class GUIDockBase
    {
        public Vector4 m_size = new Vector4(0,0,100, 100);
        public abstract void Draw(Vector4 rect);
    }

    public class GUIDock : GUIDockBase
    {
        public object m_target;
        public GUIDockGroup m_parent;

        private Vector4 m_color = RigelColor.Random();

        public override void Draw(Vector4 size)
        {
            m_size = size;

            //GUI.BeginGroup(size);
            //var rect = size;
            //GUI.DrawRect(rect, m_color);

            //GUILayout.Text("dock");

            //GUI.EndGroup();
        }
    }

    public class GUIDockGroup : GUIDockBase
    {
        public enum GUIDockOrient
        {
            Horizontal,
            Vertical
        };

        public List<GUIDockBase> m_children = new List<GUIDockBase>();
        public GUIDockOrient m_orient = GUIDockOrient.Horizontal;

        public GUIDockGroup()
        {

        }

        public override void Draw(Vector4 size)
        {
            m_size = size;
            ReiszeChild();
        }

        private void ReiszeChild()
        {
            float stotal = 0;
            float stepSize = 0;
            foreach(var c in m_children)
            {
                if(m_orient == GUIDockOrient.Horizontal)
                {
                    stotal += c.m_size.Z;
                }
                else
                {
                    stotal += c.m_size.W;
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

            foreach(var c in m_children)
            {
                if (m_orient == GUIDockOrient.Horizontal)
                {
                    crect.Z =c.m_size.Z * stepSize;
                    c.Draw(crect);
                    crect.X += c.m_size.Z;
                }
                else
                {
                    crect.W = c.m_size.W * stepSize;
                    c.Draw(crect);
                    crect.Y += c.m_size.W;
                }
            }
        }

        public void AddDock(GUIDock dock)
        {
            m_children.Add(dock);
        }

    }



    public class GUIDockManager
    {
        private GUIDockGroup m_maingroup;

        public GUIDockManager()
        {
            m_maingroup = new GUIDockGroup();
            m_maingroup.AddDock(new GUIDock());
            m_maingroup.AddDock(new GUIDock());
        }

        public void Update(Vector4 arearect)
        {
            GUI.BeginGroup(arearect);
            //GUILayout.BeginArea(arearect);
            GUILayout.SetLineHeight(20);

            m_maingroup.Draw(arearect);

            GUILayout.RestoreLineHeight();
            //GUILayout.EndArea();
            GUI.EndGroup();
        }
    }
}
