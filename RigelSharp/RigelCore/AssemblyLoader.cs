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
        public static int LoadAssemblies(string folderpath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderpath);
            if (!dir.Exists) return 0;

            int asmcount = 0;

            var files = dir.GetFiles();
            foreach(var f in files)
            {
                if (f.Name.ToLower().StartsWith("rigel.") && f.Name.ToLower().EndsWith(".dll"))
                {
                    var asm = Assembly.LoadFile(f.FullName);
                    if (asm != null)
                    {
                        Console.WriteLine("[AsmLoad]" + asm.FullName);
                        asmcount++;
                    }
                }
            }

            return asmcount;

        }
    }
}
