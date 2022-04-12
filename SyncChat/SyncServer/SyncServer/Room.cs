using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SyncServer
{
    //룸 객체 정의
    //TODO room 인터페이스 정의 : (로비, 채팅방, 게임룸 등등..)
    public class Room
    {
        public HashSet<Socket> RoomMember { get; set; }
        public int roomNumber { get; set; }
        public int maxMember = 10;
        
        
        public void DeleteMember(Socket socket)
        {
            RoomMember.Remove(socket);
        }

        public void AddMember(int roomnum,Socket socket)
        {
            roomNumber = roomnum;
            RoomMember.Add(socket);
        }

    }
}
