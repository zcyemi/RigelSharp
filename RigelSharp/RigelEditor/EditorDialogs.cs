using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;

namespace RigelEditor
{
    internal class EditorFileSystemDialog : GUIDialog
    {
        private string m_targetpath = "";

        public EditorFileSystemDialog(string title)
        {
            m_title = title;
        }

        protected override void OnDraw()
        {
            GUILayout.Button("ddd");


            var offset = GUILayout.CurrentLayout.Offset;

            GUILayout.Space(m_size.Y - offset.Y - 50);
            m_targetpath = GUILayout.TextInput("FileName", m_targetpath);
            GUILayout.BeginHorizontal();
            GUILayout.Indent((int)(m_size.X - 243));
            if (GUILayout.Button("Open", GUIOption.Width(120))) OnOpen();
            if (GUILayout.Button("Cancel", GUIOption.Width(120))) OnCancel();
            GUILayout.EndHorizontal();

        }

        private void OnOpen()
        {
            OnDestroy();
        }

        private void OnCancel()
        {
            OnDestroy();
        }
    }

    internal class EditorTestWindowedDialog : GUIWindowedDialog
    {
        public EditorTestWindowedDialog() : base(true, true, true)
        {

        }

        protected override void OnDraw()
        {

        }
    }

}
