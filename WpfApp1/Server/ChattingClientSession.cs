using System.Net;
using Core;

namespace Server
{
    using ByteSegment = System.ArraySegment<byte>;

    public class ChattingClientSession : Session
    {
        private ChattingManager _chatManager;
        protected override void OnConnected(EndPoint endPoint)
        {
        }

        protected override void OnDisconnected(EndPoint endPoint)
        {
        }

        protected override int OnReceived(ByteSegment buffer)
        {
        }

        protected override void OnSent(ByteSegment buffer)
        {
        }
    }
}