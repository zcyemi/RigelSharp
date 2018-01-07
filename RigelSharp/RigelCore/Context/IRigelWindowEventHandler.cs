using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rigel
{
    public interface IRigelWindowEventHandler
    {
        event EventHandler OnDragDrop;
        event EventHandler OnDragEnter;
        event EventHandler OnMouseWheel;
        event EventHandler OnMouseClick;
        event EventHandler OnMouseDoubleClick;
        event EventHandler OnMouseMove;
        event EventHandler OnMouseDown;
        event EventHandler OnMouseUp;
        event EventHandler OnKeyPress;
        event EventHandler OnKeyDown;
        event EventHandler OnKeyUp;

        event EventHandler OnUserResize;
    }
}
