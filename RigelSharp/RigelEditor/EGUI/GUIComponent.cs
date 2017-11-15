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

        public abstract void Draw(GUIEvent guievent);

    }

    public class GUIMessageBox:IGUIComponent
    {
        public string title = "MessageBox";
        public string info = "No Content";
        public string buttonConfirm = "Confirm";
        public string buttonCancel = "Cancel";

        private Action m_callbackConfirm;
        private Action m_callbackCancel;


        public GUIMessageBox(string title,string info,Action cbconfirm = null,Action cbcancel = null,string btnconfirm = null,string btncancel = null)
        {
            this.title = title ?? this.title;
            this.info = info ?? this.info;

            m_callbackConfirm = cbconfirm;
            m_callbackCancel = cbcancel;
            buttonConfirm = btnconfirm ?? buttonConfirm;
            buttonCancel = btncancel ?? buttonCancel;
        }

        public override void Draw(GUIEvent guievent)
        {
            GUI.BeginGroup(new Vector4(200, 200, 500, 200), null,true);
            var rect = new Vector4(0, 0, 500, 200);
            //background
            GUI.DrawRect(rect);
            //header
            rect.W = 25;
            GUI.DrawRect(rect);
            GUI.Label(rect, title);

            //info
            rect.W = 125;
            rect.Y = 25;
            GUI.Label(rect, info);

            //buttons
            rect.Y += 125;
            rect.W = 25;
            rect.X = 300;
            rect.Z = 70;
            if (GUI.Button(rect, buttonConfirm))
            {
                if (m_callbackConfirm != null) m_callbackConfirm.Invoke();

                Console.WriteLine(title + " confirm");
                Distroy = true;
            }
            rect.X += 80;
            if (GUI.Button(rect, buttonCancel))
            {
                if (m_callbackCancel != null) m_callbackCancel.Invoke();
                Console.WriteLine(title + " cancel");
                Distroy = true;
            }
            GUI.EndGroup();
        }

        public override string ToString()
        {
            return "[MessageBox]" + title;
        }
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
                        RigelUtility.Log("[Error] add menuItem duplicate:" + path);
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
                            RigelUtility.Log("[Error] add menuItem incompatable:" + path);
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
