using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Engine
{
    public interface IGameEntry
    {
        void OnStart();

        void OnUpdate();

        void OnDestroy();


    }
}
