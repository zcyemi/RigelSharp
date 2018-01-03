using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel
{
    public interface IRigelWindow
    {
        bool AllowResizing { get; set; }
        bool Fullscreen { get; set; }


        void Run(Action update);

        void DestroyWindow();
    }
}
