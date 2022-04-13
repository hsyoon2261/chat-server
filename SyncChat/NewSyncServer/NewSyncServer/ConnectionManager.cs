using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace NewSyncServer
{
    public class ConnectionManager : IDisposable
    {
        public bool isConnected = true;
        private Socket _socket;
        private ClientSession _clientSession;
        private PacketProcess _packetGenerator = new PacketProcess();


        public void Init(Socket socket, Func<ClientSession> sessionFactory)
        {
            _clientSession = sessionFactory.Invoke();
            _socket = socket;
            _clientSession.Register(socket);
            var th = new Thread(new ThreadStart(Run));
            th.Start();
            th.Join();
        }

        void Run()
        {
            while (isConnected)
            {
                var recvBuff = new byte[1024];

                try
                {
                    while (true)
                    {
                        var recvBytes = _socket.Receive(recvBuff);
                        
                        if (recvBytes != 0)
                        {
                            _packetGenerator.Init(_clientSession, recvBytes, recvBuff);
                        }
                        else
                        {
                            Console.WriteLine("소켓을 엑세스하는 동안 오류가 발생했습니다.");
                            isConnected = false;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("클라이언트쪽에서 접속을 끊었습니다.");
                    isConnected = false;
                }
            }
            Destroy();
        }


        void Destroy()
        {
            //_packetGenerator.Kick(_clientSession);
            _clientSession = null;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

        }
        public void Dispose()
        {
        }
    }

}