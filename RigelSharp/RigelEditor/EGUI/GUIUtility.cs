using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public static class GUIUtility
    {
        /// <summary>
        /// rect is relative to group
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        internal static bool RectClip(ref Vector4 rect, Vector4 group)
        {
            rect.X = MathUtil.Clamp(rect.X, 0, group.Z);
            rect.Y = MathUtil.Clamp(rect.Y, 0, group.W);

            rect.Z = MathUtil.Clamp(rect.Z, 0, group.Z - rect.X);
            rect.W = MathUtil.Clamp(rect.W, 0, group.W - rect.Y);

            rect.X += group.X;
            rect.Y += group.Y;

            if (rect.Z < 1.0f || rect.W < 1.0f) return false;
            return true;
        }

        internal static bool RectContainsCheck(Vector2 pos, Vector2 size, Vector2 point)
        {
            if (point.X < pos.X || point.X > pos.X + size.X) return false;
            if (point.Y < pos.Y || point.Y > pos.Y + size.Y) return false;
            return true;
        }

        internal static bool RectContainsCheck(Vector4 rect, Vector2 point)
        {
            if (point.X < rect.X || point.X > rect.X + rect.Z) return false;
            if (point.Y < rect.Y || point.Y > rect.Y + rect.W) return false;
            return true;
        }


        public static Vector4 Padding(this Vector4 v,float offset)
        {
            v.X += offset;
            v.Y += offset;
            v.Z -= offset;
            v.W -= offset;
            return v;
        }
    }
}
