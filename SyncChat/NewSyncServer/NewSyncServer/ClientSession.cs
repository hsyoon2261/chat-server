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
        public Mode mode = Mode.None;
        public Socket _socket;
        private string userName;
        
        public void Register(Socket socket)
        {
            _socket = socket;
            mode = Mode.Lobby;
        }

        public void GetName(string userName)
        {
            this.userName = userName;
        }

    }
}