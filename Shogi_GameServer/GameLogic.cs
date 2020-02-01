using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}
