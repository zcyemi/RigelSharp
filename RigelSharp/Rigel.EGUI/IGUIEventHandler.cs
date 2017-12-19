using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.EGUI
{
    public interface IGUIEventHandler
    {
        event Action<GUIEvent> EventUpdate;
    }
}
