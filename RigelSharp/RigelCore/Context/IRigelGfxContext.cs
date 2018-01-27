using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.Rendering;

namespace Rigel.Context
{
    public interface IRigelGfxContext
    {
        GraphicsContextBase CreateGraphicsContext(object initobj);
    }
}
