using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SyncServer
{
    class SendManager
    {
        public static void BroadCast(byte[] msg)
        {
            
        }

        public static void BroadCast(HashSet<Socket> set,Socket socket, short id, byte[] msg,int flag)
        {
            byte[] sendmsg = PacketToBytes.Make(id, msg);
            foreach (Socket s in set)
            {
                if (s == null)
                    continue;
                if (s == socket && flag == 1)
                    continue;
                s.Send(sendmsg);
                
            }
        }

        public static void BroadCast(Socket socket,short id,byte[] msg)
        {
            byte[] sendmsg = PacketToBytes.Make(id, msg);
            socket.Send(sendmsg);

        }
    }
}