using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelAppLoader
{
    public class RigelEngineApplication
    {
        private static RigelEngineApplication s_instance = new RigelEngineApplication();
        public static RigelEngineApplication Instance
        {
            get
            {
                return s_instance;
            }
        }

        private RigelEngineApplication()
        {
            Init();
        }

        private void Init()
        {

        }

        public void Run()
        {

        }
    }
}
