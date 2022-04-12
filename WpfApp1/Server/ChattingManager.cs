using System.Collections.Generic;

namespace Server
{
    public class ChattingManager
    {
        public static ChattingManager Instance => _instance;
        private static readonly ChattingManager _instance = new ChattingManager();

        private ChattingManager()
        {
        } //생성자 봉인? 이게뭐지 

        public HashSet<ChattingClientSession> ClientSessionList { get; } = new HashSet<ChattingClientSession>(4);

        public Dictionary<int, HashSet<ChattingClientSession>> ChatMember { get; } =
            new Dictionary<int, HashSet<ChattingClientSession>>(4);
        
        
        
    }
}