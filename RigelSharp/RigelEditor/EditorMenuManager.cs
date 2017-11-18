using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;

namespace RigelEditor
{

    public class EditorMenuManager :Singleton<EditorMenuManager>
    {

    }



    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true,Inherited =false)]
    public class EditorMenuItemAttribute : Attribute
    {
        public string MenuPath;
        public EditorMenuItemAttribute(string path)
        {
            MenuPath = path;
        }
    }
}
