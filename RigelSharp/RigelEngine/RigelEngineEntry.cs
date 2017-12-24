using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Rigel;

namespace RigelEngine
{
    internal static class RigelEngineEntry
    {
        static void Run(string dataPath)
        {
            Console.WriteLine("[Entry]Run: " + dataPath);

            int asmcount = AssemblyLoader.LoadAssemblies(Path.Combine(dataPath,"Assembly"));
            Console.WriteLine("[Entry] AsmLoad:" + asmcount);
        }
    }
}
