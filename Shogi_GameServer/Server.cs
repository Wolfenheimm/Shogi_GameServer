using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Shogi_GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        private static TcpListener tcpListener;
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static int[] clientLogoff = new int[] {0,0,0};
        public static int totalLoggedIn = 0;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static Dictionary<int, Piece> pieces = new Dictionary<int, Piece>();
        public static int playerTurn = 1;
        private static IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        public static int dataBufferSize = 4096;
        public static string ip = "127.0.0.1";
        public static int port = 26950;
        public static int myId = 0;
        public static TCP tcp;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(localAddr, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            //HostStart();
            //tcp.Connect();
            Console.WriteLine($"Server started on {Port}.");
        }


        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from { _client.Client.RemoteEndPoint}... ");

            if (totalLoggedIn != 2)
            {
                for (int i = 1; i <= MaxPlayers; i++)
                {
                    if (clients[i].tcp.socket == null)
                    {
                        clients[i].tcp.Connect(_client);
                        return;
                    }
                }
            }

            Console.WriteLine($"{ _client.Client.RemoteEndPoint} failed to connect: Server is full or game has not finished yet.");
        }

        private static void HostStart()
        {
            tcp = new TCP();
        }

        public void ConnectToServer()
        {
            tcp.Connect();
        }

        public void Disconnect()
        {
            tcp.Disconnect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(ip, port, ConnectCallback, socket);
            }

            public void Disconnect()
            {
                socket.Close();
                stream.Close();
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        //TODO: disconnect
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {_ex}");
                    //TODO: disconnect
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](myId, _packet);
                        }
                    });

                    _packetLength = 0;

                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public static void InitializeServerData()
        {
            for(int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.logIn, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.movePiece, ServerHandle.MovePiece },
                {(int)ClientPackets.promotePiece, ServerHandle.PromotePiece },
                {(int)ClientPackets.logOff, ServerHandle.LogOff },
                {(int)HostPackets.instance, ServerHandle.WelcomeFromHost },
            };

            Console.WriteLine("Initialized packets.");
        }

        public static void InitializeBoard()
        {
            int key = 0;
            int playerOne = 1;
            int playerTwo = 2;

            // Gyokusho
            Piece Gyokusho_A = new Piece(key, playerOne, "Gyokusho", 4, 0);
            Console.WriteLine($"Player 1 Gyokusho piece added on key value {key}");
            pieces.Add(key++, Gyokusho_A);

            Piece Gyokusho_B = new Piece(key, playerTwo, "Gyokusho", 4, 8);
            Console.WriteLine($"Player 1 Gyokusho piece added on key value {key}");
            pieces.Add(key++, Gyokusho_B);            

            // Fuhyo
            for (int x = 0; x < 9; x++)
            {
                Piece Fuhyo_A = new Piece(key, playerOne, "Fuhyo", x, 2);
                Console.WriteLine($"Player 1 Fuhyo piece added on key value {key}");
                pieces.Add(key++, Fuhyo_A);                

                Piece Fuhyo_B = new Piece(key, playerTwo, "Fuhyo", x, 6);
                Console.WriteLine($"Player 2 Fuhyo piece added on key value {key}");
                pieces.Add(key++, Fuhyo_B);                
            }

            // Kinsho
            for (int x = 3; x <= 5; x += 2)
            {
                Piece Kinsho_A = new Piece(key, playerOne, "Kinsho", x, 0);
                Console.WriteLine($"Player 1 Kinsho piece added on key value {key}");
                pieces.Add(key++, Kinsho_A);                

                Piece Kinsho_B = new Piece(key, playerTwo, "Kinsho", x, 8);
                Console.WriteLine($"Player 1 Kinsho piece added on key value {key}");
                pieces.Add(key++, Kinsho_B);                
            }

            // Ginsho
            for (int x = 2; x <= 6; x += 4)
            {
                Piece Ginsho_A = new Piece(key, playerOne, "Ginsho", x, 0);
                Console.WriteLine($"Player 1 Ginsho piece added on key value {key}");
                pieces.Add(key++, Ginsho_A);                

                Piece Ginsho_B = new Piece(key, playerTwo, "Ginsho", x, 8);
                Console.WriteLine($"Player 1 Ginsho piece added on key value {key}");
                pieces.Add(key++, Ginsho_B);              
            }

            // Keiema
            for (int x = 1; x <= 7; x += 6)
            {
                Piece Keima_A = new Piece(key, playerOne, "Keima", x, 0);
                Console.WriteLine($"Player 1 Keima piece added on key value {key}");
                pieces.Add(key++, Keima_A);                

                Piece Keima_B = new Piece(key, playerTwo, "Keima", x, 8);
                Console.WriteLine($"Player 1 Keima piece added on key value {key}");
                pieces.Add(key++, Keima_B);                
            }

            // Kyosha
            for (int x = 0; x <= 8; x += 8)
            {
                Piece Kyosha_A = new Piece(key, playerOne, "Kyosha", x, 0);
                Console.WriteLine($"Player 1 Kyosha piece added on key value {key}");
                pieces.Add(key++, Kyosha_A);                

                Piece Kyosha_B = new Piece(key, playerTwo, "Kyosha", x, 8);
                Console.WriteLine($"Player 1 Kyosha piece added on key value {key}");
                pieces.Add(key++, Kyosha_B);                
            }

            // Kakugyo
            Piece Kakugyo_A = new Piece(key, playerOne, "Kakugyo", 1, 1);
            Console.WriteLine($"Player 1 Kakugyo piece added on key value {key}");
            pieces.Add(key++, Kakugyo_A);

            Piece Kakugyo_B = new Piece(key, playerTwo, "Kakugyo", 7, 7);
            Console.WriteLine($"Player 1 Kakugyo piece added on key value {key}");
            pieces.Add(key++, Kakugyo_B);

            // Hisha
            Piece Hisha_A = new Piece(key, playerOne, "Hisha", 7, 1);
            Console.WriteLine($"Player 1 Hisha piece added on key value {key}");
            pieces.Add(key++, Hisha_A);

            Piece Hisha_B = new Piece(key, playerTwo, "Hisha", 1, 7);
            Console.WriteLine($"Player 1 Hisha piece added on key value {key}");
            pieces.Add(key++, Hisha_B);
        }
    }
}
