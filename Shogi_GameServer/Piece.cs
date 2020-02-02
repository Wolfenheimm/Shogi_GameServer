using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class Piece
    {
        public int key;
        public int id;
        public string pieceName;
        public int posX;
        public int posY;
        public int finPosX;
        public int finPosY;

        public Piece(int key, int _clientId, string _pieceName, int _posX, int _posY)
        {
            id = _clientId;
            pieceName = _pieceName;
            posX = _posX;
            posY = _posY;
        }

        public Piece(int key, int _clientId, string _pieceName, int _posX, int _posY, int _finPosX, int _finPosY)
        {
            id = _clientId;
            pieceName = _pieceName;
            posX = _posX;
            posY = _posY;
            finPosX = _finPosX;
            finPosY = _finPosY;
        }
    }
}
