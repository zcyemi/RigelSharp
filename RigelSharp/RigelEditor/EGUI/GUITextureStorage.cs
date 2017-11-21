using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SharpDX;
using RigelCore;
using RigelCore.Alg;
using RigelCore.Rendering;

namespace RigelEditor.EGUI
{
    public class GUITextureDraw
    {
        internal RenderTextureIdentifier m_texture;
        internal Vector4 m_rect;
        internal float m_depth;
        private long m_hash;

        public bool Checked = false;

        public long Hash { get { return m_hash; } }

        public void Reset()
        {
            m_texture = null;
            m_depth = 0;
            Checked = false;
            m_hash = 0;
        }

        public void SetValue(RenderTextureIdentifier rt, Vector4 rect, float depth)
        {
            m_texture = rt;
            m_rect = rect;
            m_depth = depth;

            m_hash = GetHash(rect, depth);
        }

        public GUITextureDraw(RenderTextureIdentifier rt, Vector4 rect, float depth)
        {
            m_texture = rt;
            m_rect = rect;
            m_depth = depth;

            m_hash = GetHash(rect,depth);
        }

        public static long GetHash(Vector4 rect,float depth)
        {
            MemoryStream ms = new MemoryStream(20);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(rect.X);
            bw.Write(rect.Y);
            bw.Write(rect.Z);
            bw.Write(rect.W);
            bw.Write(depth);

            long hash = RigelCore.Alg.HashFunction.RSHash(ms.ToArray());
            bw.Close();
            ms.Dispose();

            return hash;
        }

    }

    internal class GUITextureStorage
    {
        public Dictionary<RenderTextureIdentifier, List<GUITextureDraw>> m_textureStorage = new Dictionary<RenderTextureIdentifier, List<GUITextureDraw>>();
        public bool m_changed = false;

        private Stack<GUITextureDraw> m_objpool = new Stack<GUITextureDraw>();


        private List<RigelEGUIVertex> m_bufferData = new List<RigelEGUIVertex>();
        public List<RigelEGUIVertex> BufferData { get { return m_bufferData; } }

        public bool Changed { get { return m_changed; } }

        public void OnFrame()
        {
            m_changed = false;

            var lists = new List<List<GUITextureDraw>>(m_textureStorage.Values);
            foreach (var list in lists)
            {
                foreach(var draw in list)
                {
                    draw.Checked = false;
                }
            }
        }

        public bool EndFrame()
        {
            var lists = new List<List<GUITextureDraw>>(m_textureStorage.Values);
            foreach(var list in lists)
            {
                for (int i= list.Count - 1; i >= 0; i--)
                {
                    var draw = list[i];
                    if (!draw.Checked)
                    {
                        list.Remove(draw);
                        draw.Reset();
                        m_objpool.Push(draw);

                        m_changed = true;
                    }
                }
            }

            if (m_changed)
            {
                GenVertexBuffer();
                Console.WriteLine("tex draw changed!");
            }

            return m_changed;
        }

        public void AddDraw(RenderTextureIdentifier identifier,Vector4 rect,float depth)
        {
            if (m_textureStorage.ContainsKey(identifier))
            {
                long nhash = GUITextureDraw.GetHash(rect, depth);

                var drawlist = m_textureStorage[identifier];
                foreach(var draw in drawlist)
                {
                    if(draw.Hash == nhash)
                    {
                        draw.Checked = true;
                        return;
                    }
                }

                var newdraw = GetNewGUITextureDraw(identifier, rect, depth);
                newdraw.Checked = true;
                drawlist.Add(newdraw);
                m_changed = true;
            }
            else
            {
                m_changed = true;
                var drawlist = new List<GUITextureDraw>();

                var newdraw = GetNewGUITextureDraw(identifier, rect, depth);
                newdraw.Checked = true;
                drawlist.Add(newdraw);
                m_textureStorage.Add(identifier, drawlist);
            }
        }

        private GUITextureDraw GetNewGUITextureDraw(RenderTextureIdentifier rt, Vector4 rect, float depth)
        {
            if (m_objpool.Count == 0)
            {
                return new GUITextureDraw(rt, rect, depth);
            }
            else
            {
                var draw = m_objpool.Pop();
                draw.SetValue(rt, rect, depth);
                return draw;
            }
        }

        public void GenVertexBuffer()
        {
            m_bufferData.Clear();
            foreach (var list in m_textureStorage.Values)
            {
                foreach (var draw in list)
                {
                    var rect = draw.m_rect;

                    var vert = new RigelEGUIVertex();
                    vert.Position = new Vector4(rect.X, rect.Y, draw.m_depth, 1);
                    vert.UV = Vector2.Zero;
                    vert.Color = Vector4.One;
                    m_bufferData.Add(vert);

                    vert.Position.Y += rect.W;
                    vert.UV.Y = 1;
                    m_bufferData.Add(vert);

                    vert.Position.X += rect.Z;
                    vert.UV.X = 1;
                    m_bufferData.Add(vert);

                    vert.Position.Y -= rect.W;
                    vert.UV.Y = 0;
                    m_bufferData.Add(vert);
                }
            }

        }
    }
}
