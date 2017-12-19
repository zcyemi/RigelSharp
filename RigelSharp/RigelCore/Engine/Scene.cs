using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.Engine
{
    public class Scene
    {
        private static Scene s_currentScene = new Scene();
        public static Scene Current { get { return s_currentScene; } }

        
    }
}
