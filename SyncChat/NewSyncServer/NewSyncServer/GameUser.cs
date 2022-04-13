using System.Collections.Generic;
using System.Net.Sockets;

namespace NewSyncServer
{
    
    public class GameUser
    {
        public static List<Socket> TotalMember = new List<Socket>();

        public static List<Socket> LoginMember = new List<Socket>();



        public static void Insert(Socket socket, ClientSession.Mode mode)
        {
            
        }

        public static void Delete(Socket socket, ClientSession.Mode mode)
        {
            
        }

        public static void Switch(Socket socket, ClientSession.Mode now, ClientSession.Mode target)
        {
            
        }

        public static void Destroy(Socket socket, ClientSession.Mode mode)
        {
            
        }
        
    }
}