using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUICtx
    {
        public GUIStackValue<Vector4> Color = new GUIStackValue<Vector4>(GUIStyle.Current.Color);
        public GUIStackValue<Vector4> BackgroundColor = new GUIStackValue<Vector4>(GUIStyle.Current.BackgroundColor);

        /// <summary>
        /// groupStack contains currentGroup
        /// </summary>
        //public Stack<Vector4> groupStack = new Stack<Vector4>();
        public Stack<GUIGroupInfo> groupStack = new Stack<GUIGroupInfo>();
        public GUIGroupInfo currentGroup;
        public Vector4 baseRect;
        ///// <summary>
        ///// relative to parent group
        ///// </summary>
        //public Vector4 currentGroup;
        ///// <summary>
        ///// relative to baseRect
        ///// </summary>
        //public Vector4 currentGroupAbsolute;

        public Stack<Vector4> areaStack = new Stack<Vector4>();
        public Stack<GUILayoutInfo> layoutStack = new Stack<GUILayoutInfo>();
        /// <summary>
        /// relative to baseRect
        /// </summary>
        public Vector4 currentArea;
        public GUILayoutInfo currentLayout;
        public Vector4 GetNextDrawPos()
        {
            var rect = currentArea;
            rect.X += currentLayout.Offset.X;
            rect.Y += currentLayout.Offset.Y;

            return rect;
        }

        public Stack<int> depthLayer = new Stack<int>();


        public float s_depthStep = 0.0001f;

        public RigelFont Font { get; set; }

        //component
        public Stack<IGUIComponent> componentStack = new Stack<IGUIComponent>();


        //objpool
        internal GUIObjPool<GUIObjScrollBar> poolSrollbar = new GUIObjPool<GUIObjScrollBar>();

        public void Frame(GUIEvent guievent, int width, int height)
        {
            RigelUtility.Assert(groupStack.Count == 0);
            RigelUtility.Assert(areaStack.Count == 0);

            RigelUtility.Assert(layoutStack.Count == 0);
            RigelUtility.Assert(depthLayer.Count == 0);

            GUI.Event = guievent;
            baseRect = new Vector4(0, 0, width, height);
            currentGroup.Rect = baseRect;
            currentGroup.Absolute = baseRect;

            //layout
            currentLayout.Offset = Vector2.Zero;
            currentLayout.SizeMax = Vector2.Zero;
            currentLayout.Verticle = true;

            currentArea = baseRect;


            //ObjPools
            poolSrollbar.OnFrame();
        }

    }


    public struct GUIGroupInfo
    {
        public Vector4 Rect;
        public Vector4 Absolute;
    }


    public class GUIDrawTarget
    {
        public List<RigelEGUIVertex> bufferRect;
        public List<RigelEGUIVertex> bufferText;
        public float depth;

        public GUIDrawTarget(float depth)
        {
            this.depth = depth;
            bufferRect = new List<RigelEGUIVertex>();
            bufferText = new List<RigelEGUIVertex>();
        }
    }

    public class GUIStackValue<T>
    {
        private Stack<T> m_stack = new Stack<T>();
        public T Value { get; private set; }

        public GUIStackValue(T defaultval)
        {
            Value = defaultval;
            m_stack.Push(Value);
        }

        public void Set(T v)
        {
            m_stack.Push(Value);
            Value = v;
        }
        public void Restore()
        {
            Value = m_stack.Pop();
        }

        public T Peek()
        {
            return m_stack.Peek();
        }

        public static implicit operator T(GUIStackValue<T> v)
        {
            return v.Value;
        }
    }


#region Immediate Objects

    internal enum GUIObjectType : byte
    {
        ScrollBar = 1,
        DragRegion = 2,
    }

    internal static class GUIUtilityInternal
    {
        public static long GetHash(Vector4 rect,GUIObjectType type)
        {
            MemoryStream ms = new MemoryStream(20);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(rect.X);
            bw.Write(rect.Y);
            bw.Write(rect.Z);
            bw.Write(rect.W);
            bw.Write((byte)type);

            long hash = RigelCore.Alg.HashFunction.RSHash(ms.ToArray());
            return hash;
        }
    }

    public class GUIObjBase
    {
        public bool Checked = false;
    }

    internal class GUIObjPool<T> where T: GUIObjBase,new ()
    {
        public Dictionary<long, T> m_objects = new Dictionary<long, T>(8);


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
                var obj = new T();
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
                if (!m_objects[k].Checked)
                {
                    m_objects.Remove(k);
                    continue;
                }
                m_objects[k].Checked = false;
            }
        }

    }

    public class GUIObjScrollBar : GUIObjBase
    {
        public Vector4 Color = RigelColor.Random();
    }


#endregion

}
