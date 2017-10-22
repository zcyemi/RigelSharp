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

        public static void NewFrame(RigelEGUICtx ctx)
        {
            s_currentCtx = ctx;
        }

        public static void EndFrame()
        {
            s_currentCtx = null;
        }


        public static void DrawRect(Vector4 rect)
        {

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
        public bool FirstGenerated;
        public int BufferRectStartPos;
        public int BufferRectEndPos;
        public int BufferTextStartPos;
        public int BufferTextEndPos;
    }

    public class RigelEditorGUIWindow
    {


        public Vector4 Rect { get; private set; }
        public bool Focused { get { return m_focused; } internal set { m_focused = value; } }

        private RigelEditorGUIWindowBufferInfo m_bufferInfo;
        private bool m_focused = false;

        internal RigelEditorGUIWindowBufferInfo InternalBufferInfo { get { return m_bufferInfo; } }

        public RigelEditorGUIWindow()
        {
            m_bufferInfo.FirstGenerated = false;
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
