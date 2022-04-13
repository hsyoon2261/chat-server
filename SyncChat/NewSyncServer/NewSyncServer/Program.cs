using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace NewSyncServer
{
    class Program
    {        
        private static IPEndPoint ipEndPoint;
        private static int PORT = 54321;

        static void Main(string[] args)
        {
            //접속경로
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAd = IPAddress.Parse("127.0.0.1");
            ipEndPoint = new IPEndPoint(ipAd, PORT);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen();
            
            //Accept시 클라와 통신 시작
            while (true)
            {
                Socket clientsocket = serverSocket.Accept();
                ConnectionManager cManager = new ConnectionManager();
                cManager.Init(clientsocket, () => new ClientSession());
                if (!cManager.isConnected)
                {
                    Console.WriteLine("왜여길들어오지?");
                    cManager.Dispose();
                }
            }
        }
        
    }
}