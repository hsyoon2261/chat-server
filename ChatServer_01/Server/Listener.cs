using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Listener
    {
        Socket _listenSocket;
        private Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;
            
            
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);
            
            SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //한번 만들어 주면 계속 재사용이 가능하다.

            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); 
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {          
            args.AcceptSocket = null;
            
            
            bool pending = _listenSocket.AcceptAsync(args);
            //pending 을 check해서 pending이 false 라면 바로 완료가 되었다는 말임.(펜딩 없이 완료)
            // 따라서 OnAccepCompleted를 바로 호출하면 된다. 
            if(pending==false)
                OnAcceptCompleted(null, args);
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //TODO SAEA 가 AcceptSocket을 던져준다. (예약 완료된걸 전달해준다 생각하면됨)
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            //재등록
            RegisterAccept(args);
        }
    }
}