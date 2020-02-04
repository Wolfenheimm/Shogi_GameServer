using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    class ServerSend
    {

        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for(int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for(int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void WelcomeToHost(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)HostPackets.instance))
            {
                _packet.Write(_toClient);
                _packet.Write(_msg);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SpawnPiece(int _toClient, Piece _piece)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPiece))
            {
                _packet.Write(_piece.pieceKey);
                _packet.Write(_piece.id);
                _packet.Write(_piece.pieceName);
                _packet.Write(_piece.posX);
                _packet.Write(_piece.posY);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void MovePiece(int _toClient, Piece _piece)
        {
            using (Packet _packet = new Packet((int)ServerPackets.movePiece))
            {
                _packet.Write(_piece.pieceKey);
                _packet.Write(_piece.id);
                _packet.Write(_piece.pieceName);
                _packet.Write(_piece.posX);
                _packet.Write(_piece.posY);
                _packet.Write(_piece.finPosX);
                _packet.Write(_piece.finPosY);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void NextTurn(int _toClient, int _playerTurn)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerTurn))
            {
                _packet.Write(_playerTurn);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SetServer(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerTurn))
            {
                _packet.Write(_msg);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void CapturePiece(int _toClient, Piece _piece)
        {
            using (Packet _packet = new Packet((int)ServerPackets.capturePiece))
            {
                _packet.Write(_piece.pieceKey);
                _packet.Write(_piece.id);
                _packet.Write(_piece.pieceName);
                _packet.Write(_piece.posX);
                _packet.Write(_piece.posY);

                SendTCPData(_toClient, _packet);
            }
        }
    }
}
