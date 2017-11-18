﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RigelCore;
using RigelCore.Engine;

namespace SampleGame
{
    public class GameEntry : IGameEntry
    {
        public void OnStart()
        {
            Console.WriteLine("Game Start");
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
            Console.WriteLine("Game End");
        }
    }
}