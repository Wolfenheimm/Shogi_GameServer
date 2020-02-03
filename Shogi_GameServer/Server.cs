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
        public static int playerTurn;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

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

        public static void InitializeServerData()
        {
            for(int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.moveset, ServerHandle.PlayerMoveSet },
                {(int)ClientPackets.logOff, ServerHandle.LogOff },
            };

            Console.WriteLine("Initialized packets.");

            playerTurn = 1;
            Console.WriteLine("Set turn to 1.");
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
