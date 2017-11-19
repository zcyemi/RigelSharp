using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;
using RigelEditor;
using RigelEditor.EGUI;

namespace RigelEditor.Project
{
    public class ProjectManager
    {


        [EditorMenuItem("File","Project/New")]
        public static void MenuItemProjectNew()
        {
            GUI.DrawComponent(new EditorFileSystemDialog("OpenFolder"));
        }

        [EditorMenuItem("File","Project/Open")]
        public static void MenuItemProjectOpen()
        {

        }
    }


}
