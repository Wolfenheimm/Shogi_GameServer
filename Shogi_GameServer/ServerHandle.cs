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
        }
        public static void PlayerMoveSet(int _fromClient, Packet _packet)
        {
            int _clientId = _packet.ReadInt();
            string _clientName = _packet.ReadString();
            string _pieceName = _packet.ReadString();
            int _clientInitX = _packet.ReadInt();
            int _clientInitY = _packet.ReadInt();
            int _clientMoveX = _packet.ReadInt();
            int _clientMoveY = _packet.ReadInt();

            Console.WriteLine($"ID: {_clientId} - User {_clientName} wishes to move piece '{_pieceName}' from (X:{_clientInitX}, Y:{_clientInitY}) to (X:{_clientMoveX}, Y:{_clientMoveY})");
        }
    }
}
