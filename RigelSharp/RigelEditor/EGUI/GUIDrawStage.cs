using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public abstract class GUIDrawStage
    {
        protected string m_stageName;
        protected GUIDrawTarget m_drawTarget;

        public GUIDrawTarget DrawTarget { get { return m_drawTarget; } }

        public int Order { get; set; }

        public GUIDrawStage(string stagename,int order = 1)
        {
            m_stageName = stagename;
            Order = order;

            m_drawTarget = new GUIDrawTarget(order);
        }



        public virtual void Draw(GUIEvent guievent)
        {
            GUI.SetDrawTarget(m_drawTarget);
        }

        public virtual void SyncBuffer(EditorGUICtx eguictx)
        {

        }
    }
}
