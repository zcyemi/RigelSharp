using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;

namespace Rigel.EGUI
{
    public enum GUIScrollType: byte
    {
        Vertical = 1,
        Horizontal =2,
        All = 3
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

            if ((ScrollType & GUIScrollType.Vertical) > 0) GUILayout.Space(ScrollPos.Y);
            if ((ScrollType & GUIScrollType.Horizontal) > 0) GUILayout.Indent(ScrollPos.X);

            return ScrollPos;
        }

        public void LateDraw()
        {
            var e = GUI.Event;

            var curoffset = GUILayout.s_ctx.currentArea.ContentMax;

            //Console.WriteLine(curoffset);

            bool overflowH = (curoffset.X - ScrollPos.X) > RectAB.Z;
            bool overflowV = (curoffset.Y - ScrollPos.Y) > RectAB.W;
            if (!overflowH && !overflowV) return;

            overflowV &= (ScrollType & GUIScrollType.Vertical) > 0;
            overflowH &= (ScrollType & GUIScrollType.Horizontal) > 0;


            float contentV = (curoffset.Y - ScrollPos.Y);
            float scrollVmax = RectAB.W - contentV;

            float ch = RectAB.W / contentV;
            float chinv = 1.0f / ch;

            bool containerContains = GUIUtility.RectContainsCheck(RectAB, e.Pointer);

            bool scrollall = overflowV && overflowH;

            if (overflowV)
            {
                var rectSBV = new Vector4(RectAB.Z - 10f, 0, 10, RectAB.W);

                bool containsThumb = false;
                float depthRestored = GUI.DepthIncrease();

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
                    ScrollPos.Y -= (int)(GUI.Event.DragOffset.Y * chinv);
                    ScrollPos.Y = Mathf.Clamp(ScrollPos.Y, scrollVmax, 0);
                    thumbActive = true;
                }
                GUILayout.Rect(rectSBV, thumbActive ? GUIStyle.Current.ColorActive : GUIStyle.Current.BackgroundColor);

                if (containerContains)
                {
                    if (!e.Used && e.EventType == RigelEGUIEventType.MouseWheel)
                    {
                        ScrollPos.Y += (int)(chinv * 12 * (e.Delta > 0 ? 1 : -1));
                        ScrollPos.Y = Mathf.Clamp(ScrollPos.Y, scrollVmax, 0);
                        e.Use();
                    }
                }
            }

            if (overflowH)
            {

                float contentH = (curoffset.X - ScrollPos.X);
                float scrollHmax = RectAB.Z - contentH;

                float contentHPercent = RectAB.Z / contentH;
                float contentHPercentInv = 1.0f / contentHPercent;

                var rectSBH = new Vector4((-ScrollPos.X) * contentHPercent, (RectAB.W - 10f), RectAB.Z * contentHPercent, 10);
                float depthRestored = GUI.DepthIncrease();

                bool thumbActive = false;
                if (!GUI.Event.Used)
                {
                    if (GUIUtility.RectContainsCheck(GUILayout.GetRectAbsolute(rectSBH), GUI.Event.Pointer))
                    {
                        thumbActive = true;
                    }
                }

                if (ScrollHorizontal.OnDrag(thumbActive))
                {
                    ScrollPos.X -= (int)(GUI.Event.DragOffset.X * contentHPercentInv);
                    ScrollPos.X = Mathf.Clamp(ScrollPos.X, scrollHmax, 0);
                    thumbActive = true;
                }
                GUILayout.Rect(rectSBH, thumbActive ? GUIStyle.Current.ColorActive : GUIStyle.Current.BackgroundColor);

                if (containerContains)
                {
                    if (!overflowV && !e.Used && e.EventType == RigelEGUIEventType.MouseWheel)
                    {
                        ScrollPos.X += (int)(contentHPercentInv * 12 * (e.Delta > 0 ? 1 : -1));
                        ScrollPos.X = Mathf.Clamp(ScrollPos.X, scrollHmax, 0);
                        e.Use();
                    }
                }
            }


        }
    }
}
