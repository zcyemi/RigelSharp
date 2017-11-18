using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace RigelEditor.EGUI
{
    public enum GUIScrollType
    {
        Vertical,
        Horizontal,
        All
    }

    public class GUIObjScrollView : GUIObjBase
    {
        public Vector4 Color = RigelColor.Random();
        public GUIDragState ScrollVertical = new GUIDragState();
        public GUIDragState ScrollHorizontal = new GUIDragState();

        private bool ScrollInit = false;
        public Vector2 ScrollPos;
        public Vector4 RectAB;
        public GUIScrollType ScrollType;

        public override void Reset()
        {
            Checked = false;
            Color = RigelColor.Random();
            ScrollInit = false;
        }

        public Vector2 Draw(Vector4 rectab, Vector2 pos, GUIScrollType scrolltype = GUIScrollType.Vertical)
        {
            RectAB = rectab;
            if (!ScrollInit)
            {
                ScrollPos = pos;
                ScrollType = scrolltype;
                ScrollInit = true;
            }

            GUILayout.Space(ScrollPos.Y);

            return ScrollPos;
        }

        public void LateDraw()
        {
            var e = GUI.Event;

            var curoffset = GUILayout.s_ctx.currentLayout.Offset;

            bool overflowH = (curoffset.X - ScrollPos.X) > RectAB.Z;
            bool overflowV = (curoffset.Y - ScrollPos.Y) > RectAB.W;
            if (!overflowH && !overflowV) return;


            float contentV = (curoffset.Y - ScrollPos.Y);
            float scrollVmax = RectAB.W - contentV;

            if (overflowV)
            {
                var rectSBV = new Vector4(RectAB.Z - 10f, 0, 10, RectAB.W);

                bool containsBar = false;
                bool containsThumb = false;
                float depthRestored = GUI.DepthIncrease();

                if (!GUI.Event.Used)
                {
                    if (GUIUtility.RectContainsCheck(GUILayout.GetRectAbsolute(rectSBV), GUI.Event.Pointer))
                    {
                        containsBar = true;
                    }
                }


                float ch = RectAB.W / contentV;
                rectSBV.W *= ch;
                rectSBV.Y = (-ScrollPos.Y) / contentV * RectAB.W;

                if (!GUI.Event.Used)
                {
                    if (GUIUtility.RectContainsCheck(GUILayout.GetRectAbsolute(rectSBV), GUI.Event.Pointer))
                    {
                        containsThumb = true;
                    }
                }

                bool thumbActive = containsThumb;


                if (ScrollVertical.OnDrag(containsThumb))
                {
                    ScrollPos.Y -= GUI.Event.DragOffset.Y;
                    ScrollPos.Y = MathUtil.Clamp(ScrollPos.Y, scrollVmax, 0);
                    thumbActive = true;
                }
                GUILayout.DrawRect(rectSBV, thumbActive ? GUIStyle.Current.ColorActive : RigelColor.White);
            }



            //update scrollpos
            if (GUIUtility.RectContainsCheck(RectAB, e.Pointer))
            {
                if (e.EventType == RigelEGUIEventType.MouseWheel)
                {
                    if (overflowV)
                    {
                        ScrollPos.Y += e.Delta > 0 ? 12 : -12;
                        ScrollPos.Y = MathUtil.Clamp(ScrollPos.Y, scrollVmax, 0);
                        e.Use();
                    }
                }
            }
        }
    }
}
