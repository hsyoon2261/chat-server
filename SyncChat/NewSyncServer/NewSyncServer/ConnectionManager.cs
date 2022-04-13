using System;
using System.Net.Sockets;
using System.Threading;

namespace NewSyncServer
{
    public class ConnectionManager : IDisposable
    {
        public bool isConnected = true;
        private Socket _socket;
        private ClientSession _clientSession;
        private List<ClientSession> 


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
                byte[] recvBuff = new byte[1024];

                try
                {
                    while (true)
                    {
                        int recvBytes = _socket.Receive(recvBuff);
                        if (recvBytes != 0)
                        {
                            byte[] readpacket = new byte[recvBytes];
                            Buffer.BlockCopy(recvBuff, 0, readpacket, 0, recvBytes);
                            var RecvPacketQueue = PacketGenerator.Receive(recvBytes,readpacket);
                            while (RecvPacketQueue.Count > 0)
                            {
                                PacketData sendpacket = RecvPacketQueue.Dequeue();
                                PacketGenerator.Send(_clientSession,sendpacket);
                            }
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
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Dispose();
        }

        public void Dispose()
        {
        }
    }
}