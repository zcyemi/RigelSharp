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
        public Vector4 WinHeaderColorFocused = RigelColor.RGBA(0, 122, 204, 255);
        public float WinHeaderHeight = 25f;

        public Vector4 MainMenuBGColor = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
        public Vector4 MainStatusBarColor = RigelColor.RGBA(0, 122, 204, 255);

        public Vector4 ButtonColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
    }
}
