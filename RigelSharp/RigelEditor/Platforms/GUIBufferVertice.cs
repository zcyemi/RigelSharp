using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;
using Rigel;
using Rigel.EGUI;

namespace RigelEditor.Platforms
{
    public class GUIBufferVertice : GUIBufferList<RigelEGUIVertex>, IGUIBuffer
    {
        public int Count
        {
            get { return m_data.Count; }
        }

        public void AddVertices(Vector4 vert, Vector4 color, Vector2 uv)
        {
            m_data.Add(new RigelEGUIVertex()
            {
                Position = vert,
                Color = color,
                UV = uv
            });
        }

        public void Clear()
        {
            m_data.Clear();
        }

        public void CopyTo(Array ary)
        {
            if (ary.GetType() != typeof(RigelEGUIVertex).MakeArrayType()) return;
            m_data.CopyTo((RigelEGUIVertex[])ary);
        }

        public void CopyTo(int index, Array ary, int arrayIndex, int count)
        {
            if (ary.GetType() != typeof(RigelEGUIVertex).MakeArrayType()) return;
            m_data.CopyTo(index, (RigelEGUIVertex[])ary, arrayIndex, count);
        }

        public void RemoveRange(int startpos, int count)
        {
            m_data.RemoveRange(startpos, count);
        }

        public float VerticesZ(int index)
        {
            return m_data[index].Position.z;
        }
    }
}
