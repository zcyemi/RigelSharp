using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor
{
    public static class EditorAPI
    {

        public static void SetMainWindowCaptionInfo(string content)
        {
            RigelEditorApp.Instance.SetCaptionInfo(content);
        }
    }
}
