using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using SharpDX;
using SharpDX.Windows;

using Rigel.Rendering;


namespace Rigel.Engine
{
    public static class GameEngine
    {
        private static RenderForm s_form;
        private static bool s_inited = false;
        private static GraphicsManager s_graphicsManager;
        private static bool s_windowed = true;

        private static IGameEntry s_gamescript = null;

        public static GraphicsManager GraphicsMgr { get { return s_graphicsManager; } }


        public static void Run()
        {

            if (s_inited) return;
            s_inited = true;

            CheckGameAssembly();
            if (s_gamescript == null) return;

            s_form = new RenderForm("RigelGame");
            s_form.IsFullscreen = true;
            s_form.Show();

            s_graphicsManager = new GraphicsManager();
            s_graphicsManager.Init(s_form);
            //init
            s_gamescript.OnStart();

            RenderLoop.Run(s_form, OnFrame);
        }

        private static void OnFrame()
        {
            s_gamescript.OnUpdate();
            s_graphicsManager.OnFrame();
        }

        public static void Exit()
        {

        }

        private static void CheckGameAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            GameScriptAttribute gamescriptAttr = null;
            foreach(var assembly in assemblies)
            {
                gamescriptAttr = Attribute.GetCustomAttribute(assembly, typeof(GameScriptAttribute)) as GameScriptAttribute;
                if (gamescriptAttr != null) break;
            }

            var scripttype = gamescriptAttr.entryType;
            if (scripttype == null) return;

            s_gamescript = Activator.CreateInstance(scripttype) as IGameEntry;
        }

        private static void LoadScriptAssembly(string AssemblyName)
        {

        }
    }
}
