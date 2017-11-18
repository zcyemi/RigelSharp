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
        /// <returns>**absolute rect**</returns>
        internal static bool RectClip(ref Vector4 rect, Vector4 group)
        {
            Vector2 rb = rect.Pos() + rect.Size();
            if (rb.X < 0 || rb.Y < 0) return false;
            if (rect.X > group.Z || rect.Y > group.W) return false;

            rect.X = MathUtil.Clamp(rect.X, 0, group.Z);
            rect.Y = MathUtil.Clamp(rect.Y, 0, group.W);
            rb.X = MathUtil.Clamp(rb.X, rect.X, group.Z);
            rb.Y = MathUtil.Clamp(rb.Y, rect.Y, group.W);

            rect.Z = rb.X - rect.X;
            rect.W = rb.Y - rect.Y;

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
            v.Z -= offset *2;
            v.W -= offset *2;
            return v;
        }

        public static Vector4 Move(this Vector4 v,Vector2 off)
        {
            v.X += off.X;
            v.Y += off.Y;
            return v;
        }

        public static Vector4 CenterPos(this Vector4 v,Vector2 size)
        {
            size = (size - v.Size()) * 0.5f;
            v.X += size.X;
            v.Y += size.Y;

            return v;
        }

        public static Vector4 Move(this Vector4 v, float offx,float offy)
        {
            v.X += offx;
            v.Y += offy;
            return v;
        }
        public static Vector4 SetSize(this Vector4 v, Vector2 size)
        {
            v.Z += size.X;
            v.W += size.Y;
            return v;
        }
        public static Vector4 SetSize(this Vector4 v, float szx, float szy)
        {
            v.Z = szx;
            v.W = szy;
            return v;
        }
    }
}
