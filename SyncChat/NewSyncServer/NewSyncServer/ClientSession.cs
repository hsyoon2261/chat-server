using System.Net.Sockets;

namespace NewSyncServer
{
    
    public class ClientSession
    {
        public enum Mode
        {
            None,
            Lobby,
            Login,
            Chat,
            
        }
        private Mode mode = Mode.None;
        private Socket _socket;
        private string userName;
        public void Register(Socket socket)
        {
            _socket = socket;
            mode = Mode.Lobby;
        }

        


        //
        // public void Process()
        // {
        //     switch (mode)
        //     {
        //         case Mode.None:
        //             ProcessNonConnection();
        //             break;
        //         case Mode.Lobby:
        //             ProcessLoby();
        //             break;
        //         case Mode.Login:
        //             ProcessLogin();
        //             break;
        //         case Mode.Chat:
        //             ProcessChat();
        //             break;
        //             
        //     }
        // Sending Fields
        //private SocketAsyncEventArgs _sendArgs;
        //private Queue<ByteSegment> _sendQueue;     // 동시 전송 방지를 위한 큐
        //private List<ByteSegment> _sendBufferList; // 묶음 전송을 위한 리스트
        //private object _sendLock;
        
        // Receiving Fields
        //private SocketAsyncEventArgs _recvArgs;
        //private ReceiveBuffer _recvBuffer;
    }
}