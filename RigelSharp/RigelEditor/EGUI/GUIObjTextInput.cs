using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public class GUIObjTextInput : GUIObjBase
    {
        public bool Focused = false;

        public override void Reset()
        {
            Checked = false;
            Focused = false;
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
            var rectab = GUILayout.GetRectAbsolute(rect);


            if (GUIUtility.RectContainsCheck(rectab, GUI.Event.Pointer))
            {
                if (GUI.Event.IsMouseActiveEvent())
                {
                    Focused = true;
                }
            }
            else
            {
                if (GUI.Event.IsMouseActiveEvent())
                {
                    Focused = false;
                }
            }

            int labelwidth = 0;
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                labelwidth = (int)(rect.Z * 0.4f);
                labelwidth = MathUtil.Clamp(labelwidth, 40, 100);
                GUILayout.Text(label, null,GUIOption.Width(labelwidth));

                GUILayout.Indent(10);
            }


            GUILayout.DrawRectOnFlow(new Vector2(GUILayout.SizeRemain.X,GUILayout.s_svLineHeight),Focused ? GUIStyle.Current.BackgroundColorS: GUIStyle.Current.BackgroundColor);

            if (Focused)
            {
                GUILayout.Text(content, GUIStyle.Current.ColorActiveD);
            }
            else
            {
                GUILayout.Text(content);
            }
            
            GUILayout.EndHorizontal();

            return content;
        }
    }
}
