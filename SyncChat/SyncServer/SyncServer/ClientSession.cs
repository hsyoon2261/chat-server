using System.Net.Sockets;

namespace SyncServer
{
    //여기 룸 객체를 가지고있고 채팅 칠때는 여기서 건들까? 아니야 숫자만 넘기고 룸객체는 룸매니저에서 가지고있자. socket인자있으니까.
    public class ClientSession
    {
        public int roomNum { get; set; }

        private Socket _socket;
        public string id { get; set; }
        public ClientSession(Socket socket)
        {
            _socket = socket;
        }
        
    }
}