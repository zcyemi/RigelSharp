using SharpDX;
using System;
using System.Collections.Generic;

namespace RigelEditor.EGUI
{
    public class GUIMenu
    {

    }


    public abstract class IGUIComponent
    {
        public int BufferRectStart;
        public int BufferRectEnd;
        public int BufferTextStart;
        public int BufferTextEnd;

        public int BufferRectCount { get { return BufferRectEnd - BufferRectStart; } }
        public int BufferTextCount { get { return BufferTextEnd - BufferTextStart; } }

        public bool InitDrawed = false;
        public bool Distroy { get; internal set; }

        public void Show()
        {
            GUI.DrawComponent(this);
        }

        public void OnDistroy()
        {
            Distroy = true;
        }

        public abstract void Draw(GUIEvent guievent);

    }

    

    public class GUIDialog:IGUIComponent
    {
        public string title;
        public Action<GUIEvent> ongui;

        public override void Draw(GUIEvent guievent)
        {

        }
    }

    public class GUIOverlay: IGUIComponent
    {

        public override void Draw(GUIEvent guievent)
        {
            throw new NotImplementedException();
        }
    }


    public class GUIMenuButton:IGUIMenuItem
    {
        public string Label { get; private set; }
        public Action Method { get; private set; }


        public GUIMenuButton(string label,Action method = null)
        {
            Label = label;
            Method = method;
        }

        public void Invoke()
        {
            if (Method != null) Method.Invoke();
        }

        public bool Draw()
        {
            if (GUILayout.Button(Label))
            {
                Invoke();
                return true;
            }

            return false;
        }
    }

    public interface IGUIMenuItem
    {
        void Invoke();
        bool Draw();
    }

    public class GUIMenuList : IGUIComponent,IGUIMenuItem
    {
        private string m_label;
        public string Label { get { return m_label; } }
        private Vector4 m_basePos;

        public Dictionary<string,IGUIMenuItem> m_items = new Dictionary<string, IGUIMenuItem>();
        private GUIMenuList m_parent;

        public GUIMenuList(string label)
        {
            m_label = label;
        }

        internal void InternalSetStartPos(Vector4 baseps)
        {
            m_basePos = baseps;
            m_basePos.Z = 102;
            m_basePos.W = 200;
        }

        public void AddMenuItem(string path,Action method = null)
        {
            List<string> spath = new List<string>(path.Split('/'));
            spath.RemoveAll((a) => { return string.IsNullOrEmpty(a); });

            GUIMenuList curlist = this;
            for(int i = 0; i < spath.Count; i++)
            {
                string cpath = spath[i];
                if(i == spath.Count - 1)
                {
                    if (curlist.m_items.ContainsKey(cpath))
                    {
                        EditorUtility.Log("[Error] add menuItem duplicate:" + path);
                        break;
                    }
                    curlist.m_items.Add(cpath, new GUIMenuButton(cpath, method));
                }
                else
                {
                    if (curlist.m_items.ContainsKey(cpath))
                    {
                        var nlist = curlist.m_items[cpath] as GUIMenuList;
                        if (nlist == null)
                        {
                            EditorUtility.Log("[Error] add menuItem incompatable:" + path);
                            break;
                        }

                        curlist = nlist;
                    }
                    else
                    {
                        var nlist = new GUIMenuList(cpath);
                        nlist.m_parent = curlist;
                        m_items.Add(cpath, nlist);
                        curlist = nlist;
                    }

                }
            }
        }


        public override void Draw(GUIEvent guievent)
        {

            GUILayout.BeginArea(m_basePos);

            bool itemuse = false;
            foreach(var item in m_items)
            {
                itemuse |=item.Value.Draw();
            }

            GUILayout.EndArea();

            if (itemuse)
            {
                Distroy = true;
            }
            else
            {
                if (!guievent.Used && guievent.EventType == RigelEGUIEventType.MouseClick)
                {
                    if (m_parent != null) m_parent.Distroy = false;
                    Distroy = true;
                }
            }
        }

        public void Invoke()
        {
            GUI.DrawComponent(this);
        }

        public bool Draw()
        {
            var pos = GUILayout.s_ctx.GetNextDrawPos();
            if (GUILayout.Button(Label,GUIStyle.Current.TabBtnColor,GUIOption.Width(100)))
            {
                pos.X += GUILayout.s_ctx.currentLayout.LastDrawWidth;
                InternalSetStartPos(pos);
                Invoke();
                return true;
            }
            return false;
        }
    }
}
