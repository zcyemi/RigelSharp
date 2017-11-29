using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;
using RigelCore.Engine;

[assembly: GameScript(typeof(SampleGame.GameEntry))]
namespace SampleGame
{
    public class GameEntry : IGameEntry
    {
        public void OnStart()
        {
            Console.WriteLine("Game Start");

            TestDraw();
        }

        private void TestDraw()
        {
            GameObject g = new GameObject();
            var camera =  g.AddComponent<Camera>();
            
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
            Console.WriteLine("Game End");
        }
    }

    public static class TestClass
    {
        public static void Test()
        {
            Console.WriteLine("Test");
        }
    }

}

