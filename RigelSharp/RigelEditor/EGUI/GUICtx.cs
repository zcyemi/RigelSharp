using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SharpDX;
using RigelCore;
using RigelCore.Rendering;

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

        public Stack<GUIAreaInfo> areaStack = new Stack<GUIAreaInfo>();
        public Stack<GUILayoutInfo> layoutStack = new Stack<GUILayoutInfo>();
        /// <summary>
        /// relative to baseRect
        /// </summary>
        public GUIAreaInfo currentArea;
        public GUILayoutInfo currentLayout;
        public Vector4 GetNextDrawPos()
        {
            var rect = currentArea.Rect;
            rect.X += currentLayout.Offset.X;
            rect.Y += currentLayout.Offset.Y;

            return rect;
        }

        public Stack<int> depthLayer = new Stack<int>();
        public float s_depthStep = 0.0001f;

        public FontInfo Font { get; set; }

        //Input
        public bool InputChanged = false;

        //component
        public Stack<IGUIComponent> componentStack = new Stack<IGUIComponent>();


        //objpool
        internal GUIObjPool<GUIObjScrollView> poolSrollbar = new GUIObjPool<GUIObjScrollView>();
        internal GUIObjPool<GUIObjTextInput> poolTextInput = new GUIObjPool<GUIObjTextInput>();

        internal GUITextureStorage m_texStorage = new GUITextureStorage();
        internal GUITextureStorage TextureStorage { get { return m_texStorage; } }

        public void Frame(GUIEvent guievent, int width, int height)
        {
            EditorUtility.Assert(groupStack.Count == 0);
            EditorUtility.Assert(areaStack.Count == 0);

            EditorUtility.Assert(layoutStack.Count == 0);
            EditorUtility.Assert(depthLayer.Count == 0);

            GUI.Event = guievent;
            baseRect = new Vector4(0, 0, width, height);
            currentGroup.Rect = baseRect;
            currentGroup.Absolute = baseRect;

            //layout
            currentLayout.Offset = Vector2.Zero;
            currentLayout.SizeMax = Vector2.Zero;
            currentLayout.Verticle = true;

            currentArea.Rect = baseRect;
            currentArea.ContentMax = baseRect.Size();

            //ObjPools
            poolSrollbar.OnFrame();
            poolTextInput.OnFrame();

            m_texStorage.OnFrame();
        }

        public void EndFrame()
        {
            m_texStorage.EndFrame();
        }

        public void AddTextureDrawCall(RenderTextureIdentifier rt,Vector4 rect,float depth)
        {
            m_texStorage.AddDraw(rt, rect, depth);
        }

    }


    public struct GUIGroupInfo
    {
        public Vector4 Rect;
        public Vector4 Absolute;
    }

    public struct GUIAreaInfo
    {
        public Vector4 Rect;
        public Vector2 ContentMax;
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


}
