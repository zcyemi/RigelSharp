using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEngineLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            int exitcode = (int)EngineLoader.LoadAndRun();

            Environment.Exit(exitcode);
        }
    }
}
