#Client

## ClientProgram.cs

IPInformation = > ipInfo 생성,  Core IPInformation에 정의

Connector => 생성 Core에 정의

connector 객체로 connect
```c#
 connector.Connect(ipEndPoint, () => new ChattingServerSession());
```

ChattingServerSession 은 세션을 상속받음

Session 에 정의 되어 있는 것

```c#
        private const int TRUE = 1;
        private const int FALSE = 0;

        private Socket _socket;
        private int _isConnected;
        private SocketAsyncEventArgs _sendArgs;
        private Queue<ByteSegment> _sendQueue;     // 동시 전송 방지를 위한 큐
        private List<ByteSegment> _sendBufferList; // 묶음 전송을 위한 리스트
        private object _sendLock;
        private SocketAsyncEventArgs _recvArgs;
        private ReceiveBuffer _recvBuffer;
        protected abstract void OnConnected(EndPoint endPoint);


```

while(true) 로 계속 실행.. => winform 으로 변경


# Core 

## IPInformation 

그냥 평범한 IP 정보 저장. 

## Connector

Func<Session> _sessionFactory

###Connect
IPEndPoint endpoint 와 sessionfactory 받음

받은 endpoint 로 소켓을 만들고

sessionFactory 도 _sessionFactory에 저장. 


