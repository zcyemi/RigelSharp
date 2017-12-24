using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;


namespace Rigel
{
    public static class AssemblyLoader
    {
        public static void LoadAssemblies(string folderpath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderpath);
            if (!dir.Exists) return;
        }
    }
}
