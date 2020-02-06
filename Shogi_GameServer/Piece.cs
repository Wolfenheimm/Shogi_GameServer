using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class Piece
    {
        public int pieceKey;
        public int id;
        public string pieceName;
        public bool promoted;
        public int posX;
        public int posY;
        public int finPosX;
        public int finPosY;

        public Piece(int _key, int _clientId, string _pieceName, int _posX, int _posY)
        {
            pieceKey = _key;
            id = _clientId;
            pieceName = _pieceName;
            posX = _posX;
            posY = _posY;
        }

        /**
         * Constructor 
         */
        public Piece(int _key, int _clientId, string _pieceName, int _posX, int _posY, int _finPosX, int _finPosY)
        {
            pieceKey = _key;
            id = _clientId;
            pieceName = _pieceName;
            posX = _posX;
            posY = _posY;
            finPosX = _finPosX;
            finPosY = _finPosY;
        }
    }
}
