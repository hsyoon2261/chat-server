using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace NewSyncServer
{
    public class ConnectionManager : IDisposable
    {
        //Managing 하는 Socket 정보
        public bool isConnected = true;
        private Socket _socket;
        private ClientSession _clientSession;
        private PacketProcess _packetGenerator = new PacketProcess();


        //통신 시작시 세팅
        public void Init(Socket socket, Func<ClientSession> sessionFactory)
        {
            _clientSession = sessionFactory.Invoke();
            _socket = socket;
            _clientSession.Register(socket);
            Console.WriteLine($"익명 유저 접속 성공 {socket.RemoteEndPoint}");
            var th = new Thread(new ThreadStart(Run));
            th.Start();
            th.Join();
        }

        //쓰레드 영역
        public void Run()
        {
            while (isConnected)
            {
                var recvBuff = new byte[1024];

                try
                {
                    while (true)
                    {
                        
                        var recvBytes = _socket.Receive(recvBuff);
                        //패킷 수신하면 본격적으로 수신/송신 시작
                        if (recvBytes != 0)
                        {
                            _packetGenerator.Process(_clientSession, recvBytes, recvBuff);
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
                    Console.WriteLine("클라이언트쪽에서 접속을 끊었습니다.");  //송신 실패의 사유가 대부분임.
                    Console.WriteLine($"{e.ToString()}");
                    isConnected = false;
                }
            }
            Destroy(); //isConnected==false
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