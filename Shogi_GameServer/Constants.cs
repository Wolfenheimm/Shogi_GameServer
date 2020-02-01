using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class Constants
    {
        public const int TICKS_PER_SEC = 30;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;

        /***
         * Board Parameters
         * -> Board is 9x9 in array index format
         ***/
        public const int BOARD_WIDTH = 8;
        public const int BOARD_LENGTH = 8;
    }
}
