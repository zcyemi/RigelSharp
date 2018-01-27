using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.EGUI
{
    class GUIContent
    {
        private string m_content;

        public GUIContent(string content)
        {
            m_content = content;
        }

        public static implicit operator GUIContent(string content)
        {
            return new GUIContent(content);
        }


    }
}
