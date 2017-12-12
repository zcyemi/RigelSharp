using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public interface IGUIGraphicsBind
    {
        void SetDynamicBufferTexture(object vertexdata, int length);

        IGUIBuffer CreateBuffer();

        bool NeedRebuildCommandList { get; set; }

        void SyncDrawTarget(GUIDrawStage stage, GUIDrawTarget drawtarget);
    }
}
