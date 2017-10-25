using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class RigelEGUIStyle
    {
        public static RigelEGUIStyle Current { get { return m_currentStyle; } }
        private static RigelEGUIStyle m_currentStyle = new RigelEGUIStyle();

        public Vector4 WinBGColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector4 WinBGColorFocused = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
        public Vector4 WinHeaderColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        public float WinHeaderHeight = 20f;
    }
}
