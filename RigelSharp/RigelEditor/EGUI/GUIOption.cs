using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{

    public enum GUIOptionCheck : int
    {
        rectContains = 100,
        textOverflow = 101,
    }

    public class GUIOption
    {
        internal enum GUIOptionType : int
        {
            width = 1,
            height = 2,
            noClip = 3,

            adaptive = 4,
            adaptiveValue = 5,

            border = 6,
            expended = 7,
            grid =  8,

            checkRectContains = 100,
            checkTextOverflow = 101,
        }

        internal GUIOptionType type;
        internal Object value;

        public bool Checked()
        {
            return (bool)value;
        }

        public int IntValue { get { return (int)value; } }
        public float FloatValue { get { return (float)value; } }
        public Vector4 Vector4Value { get { return (Vector4)value; } }

        private GUIOption(GUIOptionType t,Object v)
        {
            type = t;
            value = v;
        }

        public static GUIOption AdaptiveValue()
        {
            return new GUIOption(GUIOptionType.adaptiveValue, null);
        }

        public static readonly GUIOption Expended = new GUIOption(GUIOptionType.expended, null);
        public static readonly GUIOption Adaptive = new GUIOption(GUIOptionType.adaptive, null);
        public static readonly GUIOption NoClip = new GUIOption(GUIOptionType.noClip, null);

        public static GUIOption Grid(float percentage = 1.0f)
        {
            return new GUIOption(GUIOptionType.grid, percentage);
        }

        public static GUIOption Border(Vector4? color)
        {
            return new GUIOption(GUIOptionType.border, color ?? GUIStyle.Current.ColorActiveD);
        }
        public static GUIOption Border()
        {
            return new GUIOption(GUIOptionType.border, GUIStyle.Current.ColorActiveD);
        }

        public static GUIOption Width(int width)
        {
            return new GUIOption(GUIOptionType.width, width);
        }
        public static GUIOption Height(int height)
        {
            return new GUIOption(GUIOptionType.height, height);
        }
        public static GUIOption Check(GUIOptionCheck check,bool valuedefault = false)
        {
            return new GUIOption((GUIOptionType)check, false);
        }
    }

    public static class GUIOptionExtension
    {
        public static GUIOption[] Append(this GUIOption[] opt,GUIOption newopt)
        {
            List<GUIOption> list = new List<GUIOption>(opt);
            list.Add(newopt);
            return list.ToArray();
        }
    }

}
