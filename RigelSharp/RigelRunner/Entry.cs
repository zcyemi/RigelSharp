using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Engine;

namespace RigelRunner
{
    static class Entry
    {
        [STAThread]
        static void Main(params string[] args)
        {
            //temp load the script assembly
            SampleGame.TestClass.Test();

            GameEngine.Run();
        }
    }
}
