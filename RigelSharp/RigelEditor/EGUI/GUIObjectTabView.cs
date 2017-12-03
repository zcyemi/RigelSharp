using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using RigelCore;

namespace RigelEditor.EGUI
{
    public class GUIObjectTabView : GUIObjBase
    {
        private static readonly int m_tabhederHeght = 23;

        private List<string> m_tabnames = null;
        private int m_tabindex = 0;

        

        public GUIObjectTabView()
        {
        }

        public override void Reset()
        {
            m_tabnames = null;
            m_tabindex = 0;
        }

        public int Draw(Vector4 rect,int index,List<string> tabnames,Action<int> drawFunction)
        {
            m_tabnames = tabnames;
            m_tabindex = index;

            var rectHeader = rect;
            rectHeader.W = m_tabhederHeght;

            m_tabindex = DrawTabHeader(rectHeader);

            rect.Y += m_tabhederHeght;
            rect.W -= m_tabhederHeght;
            GUILayout.BeginContainerRelative(rect, GUIStyle.Current.BackgroundColor);
            if (drawFunction != null)
                drawFunction.Invoke(index);

            GUILayout.EndContainer();

            
            return m_tabindex;
        }

        private int DrawTabHeader(Vector4 rectHeader)
        {
            GUILayout.BeginAreaR(rectHeader, GUIStyle.Current.BackgroundColorS);
            GUILayout.BeginHorizontal();

            for(int i = 0; i < m_tabnames.Count; i++)
            {
                
                if(GUILayout.Button(m_tabnames[i], m_tabindex == i ? GUIStyle.Current.ColorActiveD: GUIStyle.Current.BackgroundColorS))
                {
                    m_tabindex = i;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            return m_tabindex;
        }

    }
}
