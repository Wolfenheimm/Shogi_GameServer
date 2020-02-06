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

            //Set Login Array
            Server.clientLogoff[_fromClient] = 1;

            // Number of logged in players has incremented
            Server.totalLoggedIn++;

            // Second player connected, init and send board data
            if (_fromClient == 2)
            {
                Server.InitializeBoard();
                Server.clients[1].SendBoardData();
                Server.clients[2].SendBoardData();
            }
        }
        public static void MovePiece(int _fromClient, Packet _packet)
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
            //Console.WriteLine($"ID: {_clientId} - User {_clientName} wishes to move piece '{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientFinX}, Y:{_clientFinY})");
            Console.WriteLine($"_clientId: {_clientId} User: {_clientName} Action: MovePiece PieceName:'{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientFinX}, Y:{_clientFinY})");

            // Validate Move
            foreach (KeyValuePair<int, Piece> p in Server.pieces)
            {
                // Piece was captured
                if(p.Value.posX == _clientFinX && p.Value.posY == _clientFinY && _clientId != p.Value.id)
                {
                    Piece capturedPiece = new Piece(p.Key, p.Value.id, p.Value.pieceName, p.Value.posX, p.Value.posY);
                    ServerSend.CapturePiece(1, capturedPiece);
                    ServerSend.CapturePiece(2, capturedPiece);

                    // Update the piece dict
                    p.Value.posX = -1;
                    p.Value.posY = -1;
                }
            }

            // Update the piece dictionary on move
            Server.pieces[_pieceKey] = new Piece(_pieceKey, _clientId, _pieceName, _clientFinX, _clientFinY);
            Console.WriteLine($"{_pieceName} @ Key {_pieceKey} has been updated on the board");

            // Send move to both players
            ServerSend.MovePiece(1, piece);
            Console.WriteLine("Executed send MovePiece to Client 1");
            ServerSend.MovePiece(2, piece);
            Console.WriteLine("Executed send MovePiece to Client 2");
            //Console.WriteLine($"ID: {_clientId} - Move granted. User {_clientName} has moved '{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientFinX}, Y:{_clientFinY})");

            if (Server.playerTurn == 1)
            {
                Server.playerTurn = 2;
            }
            else
            {
                Server.playerTurn = 1;
            }

            ServerSend.NextTurn(1, Server.playerTurn);
            Console.WriteLine($"Executed [ServerSend.NextTurn] to Client 1 -> Player turn [{Server.playerTurn}]");
            ServerSend.NextTurn(2, Server.playerTurn);
            Console.WriteLine($"Executed [ServerSend.NextTurn] to Client 2 -> Player turn [{Server.playerTurn}]");
        }

        public static void PromotePiece(int _fromClient, Packet _packet)
        {
            int _pieceKey = _packet.ReadInt();

            // update piece promoted in dictionnary 
            Server.pieces[_pieceKey].promoted = true;

            ServerSend.PromotePiece(1, Server.pieces[_pieceKey]);
            ServerSend.PromotePiece(2, Server.pieces[_pieceKey]);
        }

        public static void LogOff(int _fromClient, Packet _packet)
        {
            bool _clientsRemaining = false;
            int _clientId = _packet.ReadInt();
            string _clientName = _packet.ReadString();

            Server.clients[_clientId] = null;
            Server.clientLogoff[_clientId] = 0;
            Console.WriteLine($"User '{_clientName}' with ID: {_clientId} has disconnected.");
            
            for(int x = 0; x < 3; x++)
            {
                if (Server.clientLogoff[x] == 1)
                    _clientsRemaining = true;
            }

            if (!_clientsRemaining)
            {
                Console.WriteLine($"All clients have logged off, restarting server.");
                Server.clients = new Dictionary<int, Client>();
                Server.pieces = new Dictionary<int, Piece>();
                Server.totalLoggedIn = 0;
                Server.InitializeServerData();
            }
        }

        public static void WelcomeFromHost(int _fromHost, Packet _packet)
        {
            int _HostId = _packet.ReadInt();
            string _message = _packet.ReadString();

            Console.WriteLine($"Host {_HostId} sends message ({_message}).");
        }
    }
}
