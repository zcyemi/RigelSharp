using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public interface IGUIEventHandler
    {
        event Action<GUIEvent> EventUpdate;
    }
}
