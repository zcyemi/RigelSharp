using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Rigel;

namespace RigelEngine
{
    class RigelEngineEntry
    {
        [STAThread]
        static void Main()
        {
            {
                //Temp
                Rigel.SharpDX.RigelSharpDX.Init();
                Rigel.OpenTK.RigelOpenTK.Init();
            }

            Rigel.Context.RigelContextManager.CheckContextProvider();

            RigelEngineApp.App.Init();

            RigelEngineApp.App.Run();

            RigelEngineApp.App.Destroy();
        }
    }
}
