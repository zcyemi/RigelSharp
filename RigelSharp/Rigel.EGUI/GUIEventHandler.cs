using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.Context;


namespace Rigel.EGUI
{
    public class GUIEventHandler : IGUIEventHandler
    {
        public event Action<GUIEvent> EventUpdate;

        public GUIEventHandler(IRigelWindowEventHandler windowEventHandler)
        {
            windowEventHandler.OnUserResize += (o, e) =>
            {
                EventUpdate(new GUIEvent(RigelEGUIEventType.Resize, e));
            };

            
        }
    }
}
