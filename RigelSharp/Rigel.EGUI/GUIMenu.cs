using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Rigel;

namespace Rigel.EGUI
{
    public class GUIMenuButton : IGUIMenuItem
    {
        public string Label { get; private set; }
        public Action Method { get; private set; }


        public GUIMenuButton(string label, Action method = null)
        {
            Label = label;
            Method = method;
        }

        public void Invoke()
        {
            if (Method != null) Method.Invoke();
        }

        public bool DrawMenuItem()
        {
            if (GUILayout.Button(Label, GUIOption.Width(GUIMenuList.ItemWidth)))
            {
                Invoke();
                return true;
            }

            return false;
        }

        public void SetMethod(Action method)
        {
            Method = method;
        }
    }

    public interface IGUIMenuItem
    {
        void Invoke();
        bool DrawMenuItem();
    }

    public class GUIMenuList : IGUIComponent, IGUIMenuItem
    {
        private string m_label;
        public string Label { get { return m_label; } }
        private Vector4 m_basePos;

        public Dictionary<string, IGUIMenuItem> m_items = new Dictionary<string, IGUIMenuItem>();
        private GUIMenuList m_parent = null;

        public static readonly int ItemHeight = 23;
        public static readonly int ItemWidth = 100;

        public int Order { get; set; }

        public GUIMenuList(string label)
        {
            m_label = label;
        }

        internal void InternalSetStartPos(Vector4 baseps)
        {
            m_basePos = baseps;
            m_basePos.Z = ItemWidth;
            m_basePos.W = (ItemHeight + 1) * m_items.Count;
        }

        public IGUIMenuItem GetItem(string key)
        {
            if (m_items.ContainsKey(key)) return m_items[key];
            return null;
        }

        public void AddMenuItem(string path, Action method = null)
        {
            List<string> subpath = new List<string>(path.Split('/'));
            subpath.RemoveAll((a) => { return string.IsNullOrEmpty(a); });

            if (subpath.Count == 0) return;

            string p = subpath[0];
            var list = this;
            for(int i = 0; i < subpath.Count; i++)
            {
                var key = subpath[i];
                if(i == subpath.Count - 1)
                {
                    //content

                    var listitem = list.GetItem(key);
                    if(listitem == null)
                    {
                        list.m_items.Add(key, new GUIMenuButton(key, method));
                    }
                    else
                    {
                        if(listitem is GUIMenuButton)
                        {
                            (listitem as GUIMenuButton).SetMethod(method);
                        }
                    }
                }
                else
                {
                    var listitem = list.GetItem(key);
                    if(listitem == null)
                    {
                        var newlist = new GUIMenuList(key);
                        list.m_items.Add(key, newlist);

                        list = newlist;
                    }
                    else
                    {
                        if(listitem is GUIMenuButton)
                        {
                            return;
                        }
                        else
                        {
                            list = listitem as GUIMenuList;
                        }
                    }
                }
            }
        }

        public string PrintMenuInfo(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in m_items)
            {
                if(item.Value is GUIMenuButton)
                {
                    sb.AppendLine(prefix + "/" + (item.Value as GUIMenuButton).Label);
                }
                else
                {
                    sb.Append((item.Value as GUIMenuList).PrintMenuInfo(prefix + "/" + item.Key));
                }
            }

            return sb.ToString();
        }


        public override void Draw(GUIEvent guievent)
        {

            GUILayout.BeginArea(m_basePos,null);

            GUILayout.SetLineHeight(ItemHeight);

            bool itemuse = false;
            foreach (var item in m_items)
            {
                itemuse |= item.Value.DrawMenuItem();
            }

            GUILayout.RestoreLineHeight();

            GUI.BorderA(GUI.Context.currentArea.Rect, 1, GUIStyle.Current.ColorActiveD);
            GUILayout.EndArea();

            if (itemuse)
            {
                OnDestroy();
            }
            else
            {
                if (!guievent.Used && guievent.EventType == RigelEGUIEventType.MouseClick)
                {
                    if (m_parent != null) m_parent.Destroy = false;
                    OnDestroy();
                }
            }
        }

        public void Invoke()
        {
            GUI.DrawComponent(this);
        }

        public bool DrawMenuItem()
        {
            var pos = GUILayout.s_ctx.GetNextDrawPos();
            if (GUILayout.Button(Label, GUIStyle.Current.TabBtnColor, GUIOption.Width(ItemWidth)))
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
