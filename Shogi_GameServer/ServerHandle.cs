﻿using System;
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
    }
}
