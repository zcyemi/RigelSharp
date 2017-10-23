using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public static class RigelEditorGUI
    {
        internal static RigelEGUICtx s_currentCtx = null;
        internal static RigelEditorGUIWindow s_currentWindow = null;



        public static void DrawRect(Vector4 rect,Vector4 color)
        {
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y, 0, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X, rect.Y + rect.W, 0, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X +rect.Z, rect.Y + rect.W, 0, 1),
                Color = color,
                UV = Vector2.Zero
            });
            s_currentCtx.BufferRect.Add(new RigelEGUIVertex()
            {
                Position = new Vector4(rect.X + rect.Z, rect.Y, 0, 1),
                Color = color,
                UV = Vector2.Zero
            });

        }


    }


    public class RigelEditorGUIDocker
    {

    }

    public class RigelEditorGUIDockerManager
    {

    }

    public struct RigelEditorGUILayout
    {

    }

    

    internal struct RigelEditorGUIWindowBufferInfo
    {
        public bool BufferInited;
        public int BufferRectStartPos;
        public int BufferRectEndPos;
        public int BufferTextStartPos;
        public int BufferTextEndPos;
    }

    public class RigelEditorGUIWindow
    {


        public Vector4 Rect { get; private set; }
        public bool Focused { get { return m_focused; } internal set { m_focused = value; } }

        internal RigelEditorGUIWindowBufferInfo m_bufferInfo;
        private bool m_focused = false;


        public RigelEditorGUIWindow()
        {
            m_bufferInfo.BufferInited = false;
            m_bufferInfo.BufferRectEndPos = 0;
            m_bufferInfo.BufferRectStartPos = 0;
            m_bufferInfo.BufferTextEndPos = 0;
            m_bufferInfo.BufferTextStartPos = 0;
        }

        public virtual void OnMenuBar(RigelEGUIMenu menu)
        {

        }

        public virtual void OnGUI()
        {
            
        }

        public static T GetWindow<T>() where T : RigelEditorGUIWindow, new ()
        {
            return RigelEditorGUI.s_currentCtx.FindWindowOfType<T>();
        }
    }


}
