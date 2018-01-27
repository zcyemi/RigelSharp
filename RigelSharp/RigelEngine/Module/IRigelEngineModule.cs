using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEngine.Module
{
    interface IRigelEngineModule
    {
        void Init();
        void Update();
        void Destroy();
    }
}
