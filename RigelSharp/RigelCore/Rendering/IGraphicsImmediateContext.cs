using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Rendering
{
    public interface IGraphicsImmediatelyContext
    {
        void Clear(Vector4 color);
    }
}
