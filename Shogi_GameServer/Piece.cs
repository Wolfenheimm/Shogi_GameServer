using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class Piece
    {
        public int id;
        public string pieceName;
        public int posX;
        public int posY;

        public Piece(int _clientId, string _pieceName, int _posX, int _posY)
        {
            id = _clientId;
            pieceName = _pieceName;
            posX = _posX;
            posY = _posY;
        }
    }
}
