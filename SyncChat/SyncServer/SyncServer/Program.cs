using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SyncServer
{
    class Program
    {
        private static IPEndPoint ipEndPoint;
        private static int PORT = 54321;
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAd = IPAddress.Parse("127.0.0.1");
            ipEndPoint = new IPEndPoint(ipAd, PORT);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(10);

            while (true)
            {
                Socket clientsocket = serverSocket.Accept();
                Console.WriteLine("good");
                ClientManager cManager = new ClientManager(clientsocket);
                Console.WriteLine($"client{clientsocket.RemoteEndPoint}");
                Thread th = new Thread(new ThreadStart(cManager.Run));
                th.Start();
            }

        }
    }
}