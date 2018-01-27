using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace RigelEngineLoader
{
    public enum ExitCode : int
    {
        Success = 0,
        AssemblyNotFound = 1,
        AssemblyLoadFail = 2,
        AssemblyStartFail = 3,
    }

    public static class EngineLoader
    {
        static readonly string AsmRigelEngineName = "RigelEngine.dll";
        static Assembly m_asmRigelEngine;

        public static ExitCode LoadAndRun()
        {
            string DataPath = AppContext.BaseDirectory + "Data/";
            string AssemblyPath = DataPath + "Assembly/";
            string PluginPath = DataPath + "Plugins/";

            var asmEnginePath = AssemblyPath + AsmRigelEngineName;
            if (!File.Exists(asmEnginePath))
            {
                return ExitCode.AssemblyNotFound;
            }

            Console.WriteLine(asmEnginePath);

            m_asmRigelEngine = Assembly.LoadFrom(asmEnginePath);
            if(m_asmRigelEngine == null)
            {
                return ExitCode.AssemblyLoadFail;
            }

            {
                //Init Run
                var entry = m_asmRigelEngine.GetType("RigelEngine.RigelEngineEntry");
                if(entry == null)
                {
                    return ExitCode.AssemblyStartFail;
                }

                var method = entry.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                {
                    Console.WriteLine("Method Not Found.");
                    return ExitCode.AssemblyStartFail;
                }
                method.Invoke(null, new object[] { DataPath });
            }


            return ExitCode.Success;
        }
    }
}
