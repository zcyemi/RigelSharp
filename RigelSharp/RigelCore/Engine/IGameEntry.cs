using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore.Engine
{
    public interface IGameEntry
    {
        void OnStart();

        void OnUpdate();

        void OnDestroy();


    }
}
