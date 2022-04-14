using System.Collections.Generic;
using System.Net.Sockets;

namespace NewSyncServer
{

    //전체 유저를 관리하는 클래스
    public class GameUser
    {
        private object _lock = new object();
        
        public static List<ClientSession> TotalMember = new List<ClientSession>();

        public static List<ClientSession> LoginMember = new List<ClientSession>();



        public static void Insert(ClientSession user, ClientSession.Mode mode)
        {

            switch (mode)
            {
                case ClientSession.Mode.Lobby:
                    TotalMember.Add(user);
                    break;
                case ClientSession.Mode.Login:
                    LoginMember.Add(user);
                    break;
            }
        }

        public static void Delete(ClientSession user, ClientSession.Mode mode)
        {
            
        }

        public static void Switch(ClientSession user, ClientSession.Mode now, ClientSession.Mode target)
        {
            
        }

        //Kick 전용 메서드(모든 상태 해제)
        public static void Destroy(ClientSession user, ClientSession.Mode mode)
        {
            
        }
        
    }
}