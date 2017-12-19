using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public class GUIDrawTarget
    {
        public IGUIBuffer bufferRect;
        public IGUIBuffer bufferText;
        public float depth;

        public GUIDrawTarget(float depth)
        {
            this.depth = depth;

            bufferRect =GUIInternal.GraphicsBind.CreateBuffer();
            bufferText = GUIInternal.GraphicsBind.CreateBuffer();
        }
    }
}
