using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
            if(_fromClient != _clientIdCheck) // Something went terribly wrong
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck}).");
            }

            //Send player into game
            Server.clients[_fromClient].SendIntoGame(_username);

            // Second player connected, init and send board data
            if (_fromClient == 2)
            {
                Server.InitializeBoard();
                Server.clients[1].SendBoardData();
                Server.clients[2].SendBoardData();
            }
        }
        public static void PlayerMoveSet(int _fromClient, Packet _packet)
        {
            int _pieceKey = _packet.ReadInt();
            int _clientId = _packet.ReadInt();
            string _clientName = _packet.ReadString();
            string _pieceName = _packet.ReadString();
            int _clientInitX = _packet.ReadInt();
            int _clientInitY = _packet.ReadInt();
            int _clientFinX = _packet.ReadInt();
            int _clientFinY = _packet.ReadInt();

            Piece piece = new Piece(_pieceKey, _clientId, _pieceName, _clientInitX, _clientInitY, _clientFinX, _clientFinY);
            Console.WriteLine($"ID: {_clientId} - User {_clientName} wishes to move piece '{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientFinX}, Y:{_clientFinY})");

            // Send move to both players
            ServerSend.MovePiece(1, piece);
            ServerSend.MovePiece(2, piece);
            Console.WriteLine($"ID: {_clientId} - Move granted. User {_clientName} has moved '{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientFinX}, Y:{_clientFinY})");

            // Update the piece dictionary on move
            Server.pieces[_pieceKey] = new Piece(_pieceKey, _clientId, _pieceName, _clientFinX, _clientFinY);
            Console.WriteLine($"{_pieceName} @ Key {_pieceKey} has been updated on the board");
        }
    }
}
