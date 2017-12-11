using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelEditor.EGUI;

namespace RigelEditor.Platforms
{
    public class GUIEventHandlerWindows : IGUIEventHandler
    {
        public event Action<GUIEvent> EventUpdate = delegate { };

        
    }
}
