using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Context
{
    public enum PlatformEnum : int
    {
        Windows,
        MacOS
    }

    public enum GraphicsAPIEnum :int
    {
        DirectX = 1<<0,
        OpenGL =1 <<1,
    }
}
