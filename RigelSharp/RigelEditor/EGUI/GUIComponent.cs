using SharpDX;
using System;
using System.Collections.Generic;

namespace RigelEditor.EGUI
{


    public abstract class IGUIComponent
    {
        public int BufferRectStart;
        public int BufferRectEnd;
        public int BufferTextStart;
        public int BufferTextEnd;

        public int BufferRectCount { get { return BufferRectEnd - BufferRectStart; } }
        public int BufferTextCount { get { return BufferTextEnd - BufferTextStart; } }

        public bool InitDrawed = false;
        public bool Distroy { get; internal set; }

        public void Show()
        {
            GUI.DrawComponent(this);
        }

        public void OnDistroy()
        {
            Distroy = true;
        }

        public abstract void Draw(GUIEvent guievent);

    }

    

    public class GUIDialog:IGUIComponent
    {
        public string title;
        public Action<GUIEvent> ongui;

        public override void Draw(GUIEvent guievent)
        {

        }
    }

    public class GUIOverlay: IGUIComponent
    {

        public override void Draw(GUIEvent guievent)
        {
            throw new NotImplementedException();
        }
    }


    
}
