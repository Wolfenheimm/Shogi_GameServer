using Shogi_Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi_GameServer
{
    /// <summary>
    /// ServerSend
    /// Desc: Handles sending packets of different types to the client. Server uses <see cref="SendTCPData(int, Packet)"/> to send TCP packets to the clients
    /// </summary>
    class ServerSend
    {
        /// <summary>
        /// SendTCPData
        /// Desc: Sends packets to the client
        /// Sends a <paramref name="packet"> of <typeparamref name="Packet"> to client <paramref name="toClient">
        /// <see cref="Client.TCP.SendData(Packet)"></see>
        /// </summary>
        /// <param name="toClient">Client ID, refers to player's number</param>
        /// <typeparam name="Packet">Packet meta data</typeparam>
        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        /// <summary>
        /// SendTCPDataToAll
        /// Desc: Sends packets to all clients
        /// Sends a <paramref name="packet"> of <typeparamref name="Packet"> to all clients
        /// <see cref="SendTCPData(int, Packet)"></see>
        /// </summary>
        /// <typeparam name="Packet">Packet meta data</typeparam>
        /// <param name="packet">Information passed through a packet</param>
        private static void SendTCPDataToAll(Packet packet)
        {
            for(int i = 1; i <= Server.MaxPlayers; i++)
            {
                SendTCPData(i, packet);
            }
        }

        /// <summary>
        /// Welcome
        /// Desc: Sends Welcome message to the client
        /// Sends a <paramref name="_packet"> of <typeparamref name="Packet"> containing a message to a client with <paramref name="_toClient">
        /// <see cref="ServerHandle.WelcomeReceived(int, Packet)"></see>
        /// </summary>
        /// <param name="_msg">Packet comprises message string</param>
        /// <param name="_packet"></param>
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>
        /// WelcomeToHost
        /// Desc: Sends Welcome message to the Host
        /// Sends a <paramref name="_packet"> containing a message to client <paramref name="_toClient">
        /// <see cref="ServerHandle.WelcomeFromHost(int, Packet)">Welcome message callback from Host</see>
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_msg">Packet comprises message string</param>
        public static void WelcomeToHost(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)HostPackets.instance))
            {
                _packet.Write(_toClient);
                _packet.Write(_msg);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>
        /// SpawnPlayer
        /// Desc: Sends player spawn data to the client. Player spawn provides a player class to the client containing player data.
        /// Sends a <paramref name="_player"> of <typeparamref name="Player"> to the client
        /// <see cref="ServerHandle.WelcomeReceived(int, Packet)">Player spawn data is handled when a player connects to the server</see>
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <typeparam name="Player">Player meta data</typeparam>
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

        /// <summary>
        /// SpawnPiece
        /// Desc: Spawns a shogi piece to the board according to the player's ID
        /// Sends a <paramref name="_piece"> of <typeparamref name="Piece"> to the client
        /// <see cref="Client.SendBoardData"/> and <see cref="ServerHandle.WelcomeReceived(int, Packet)">Piece spawn data is handled when both players connect and after the board initializes</see>
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_piece">Piece Parameters</param>
        /// <typeparam name="Piece">Piece meta data</typeparam>
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

        /// <summary>
        ///         MovePiece - Move shogi piece
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_piece">Piece Parameters</param>
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

        /// <summary>
        ///         NextTurn - Set the player turn
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_playerTurn">Player turn</param>
        public static void NextTurn(int _toClient, int _playerTurn)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerTurn))
            {
                _packet.Write(_playerTurn);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>
        ///         SetServer - May be related to HOST - DEPRECATED [USE MULTIPLAY]
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_msg"></param>
        public static void SetServer(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerTurn))
            {
                _packet.Write(_msg);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>
        ///         CapturePiece - Captures shogi piece
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_piece"></param>
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

        /// <summary>
        ///         PromotePiece - Promotes Shogi Piece
        /// </summary>
        /// <param name="_toClient">Client ID, refers to player's number</param>
        /// <param name="_piece"></param>
        public static void PromotePiece(int _toClient, Piece _piece)
        {
            using (Packet _packet = new Packet((int)ServerPackets.promotePiece))
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
