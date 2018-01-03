using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Context
{
    public interface IRigelWindowContext
    {
        IRigelWindow InitMainWindow(string title,bool fullscreen = false);
        IRigelWindow MainWindow { get; }

    }
}
