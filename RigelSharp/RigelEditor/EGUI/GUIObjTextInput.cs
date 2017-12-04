using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelEditor.EGUI
{
    public class GUIObjTextInput : GUIObjBase
    {
        public bool Focused = false;
        public int Pos = 0;
        public string LastString = null;
        public Vector4 PointerRect;

        public override void Reset()
        {
            Checked = false;
            Focused = false;
            Pos = 0;
            LastString = null;
            PointerRect = Vector4.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">relative</param>
        /// <param name="content"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public string Draw(Vector4 rect,string content,string label)
        {
            bool contentChanged = false;
            bool posChanged = false;

            if(content != LastString)
            {
                Pos = content.Length;
                posChanged = true;
            }
            var rectab = GUILayout.GetRectAbsolute(rect);

            if (GUI.Event.IsMouseActiveEvent())
            {
                Focused = GUIUtility.RectContainsCheck(rectab, GUI.Event.Pointer);
            }
            else if (!GUI.Event.Used)
            {
                if (Focused && GUI.Event.EventType == RigelEGUIEventType.KeyDown)
                {
                    int lastpos = Pos;
                    var newcontent = GUITextProcessor.ProcessInput(content, GUI.Event.Key,ref Pos);
                    if(content != newcontent)
                    {
                        contentChanged = true;
                        content = newcontent;
                        GUI.Context.InputChanged = true;   
                    }
                    else
                    {
                        if(lastpos != Pos)
                        {
                            posChanged = true;
                            GUI.Context.InputChanged = true;
                        }
                    }
                }
            }

            int labelwidth = 0;
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                labelwidth = (int)(rect.Z * 0.4f);
                labelwidth = MathUtil.Clamp(labelwidth, 40, 100);
                GUILayout.Text(label, (Vector4?)null, GUIOption.Width(labelwidth));

                GUILayout.Indent(10);
            }

            GUILayout.RectOnFlow(new Vector2(GUILayout.SizeRemain.X,GUILayout.s_svLineHeight),Focused ? GUIStyle.Current.BackgroundColorS: GUIStyle.Current.BackgroundColor);

            var startpos = GUILayout.CurrentLayout.Offset;
            if (Focused)
            {
                GUILayout.Text(content, GUIStyle.Current.ColorActiveD);
            }
            else
            {
                GUILayout.Text(content);
            }

            if (contentChanged || posChanged)
            {
                string substr = content.Substring(0, Pos);
                int posx = GUI.Context.Font.GetTextWidth(substr);
                PointerRect = new Vector4(startpos.X + posx + 3, startpos.Y, 1, GUILayout.s_svLineHeight);
            }
            if(Focused)
                GUILayout.Rect(PointerRect, GUIStyle.Current.Color);

            GUILayout.EndHorizontal();
            LastString = content;

            return content;
        }


        
    }
}
