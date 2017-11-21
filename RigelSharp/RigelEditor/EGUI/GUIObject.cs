using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using SharpDX;

namespace RigelEditor.EGUI
{
    internal enum GUIObjectType : byte
    {
        ScrollBar = 1,
        DragRegion = 2,
        TextInput = 3,
    }

    internal static class GUIUtilityInternal
    {
        public static long GetHash(Vector4 rect, GUIObjectType type)
        {
            MemoryStream ms = new MemoryStream(20);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(rect.X);
            bw.Write(rect.Y);
            bw.Write(rect.Z);
            bw.Write(rect.W);
            bw.Write((byte)type);

            long hash = RigelCore.Alg.HashFunction.RSHash(ms.ToArray());

            bw.Close();
            ms.Dispose();

            return hash;
        }
    }

    public abstract class GUIObjBase
    {
        public bool Checked = false;

        public abstract void Reset();
    }

    internal class GUIObjPool<T> where T : GUIObjBase, new()
    {
        public Dictionary<long, T> m_objects = new Dictionary<long, T>(8);
        private Stack<T> m_pool = new Stack<T>();


        public T Get(long hash)
        {
            if (m_objects.ContainsKey(hash))
            {
                var obj = m_objects[hash];
                obj.Checked = true;
                return m_objects[hash];
            }
            else
            {
                T obj = null;

                if (m_pool.Count == 0)
                {
                    obj = new T();
                }
                else
                {
                    obj = m_pool.Pop();
                }
                m_objects.Add(hash, obj);
                obj.Checked = true;
                return obj;


            }
        }

        public void OnFrame()
        {
            int count = m_objects.Count;
            if (count == 0) return;
            var keys = new List<long>(m_objects.Keys);

            foreach (var k in keys)
            {
                var obj = m_objects[k];

                if (!obj.Checked)
                {
                    m_objects.Remove(k);
                    obj.Reset();
                    m_pool.Push(obj);
                    continue;
                }
                obj.Checked = false;
            }
        }

    }
}
