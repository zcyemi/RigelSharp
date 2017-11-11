using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public class GUIOption
    {
        internal enum GUIOptionType
        {
            width,
            height,
        }

        internal GUIOptionType type;
        internal Object value;

        private GUIOption(GUIOptionType t,Object v)
        {
            type = t;
            value = v;
        }

        public static GUIOption Width(int width)
        {
            return new GUIOption(GUIOptionType.width, width);
        }
    }
}
