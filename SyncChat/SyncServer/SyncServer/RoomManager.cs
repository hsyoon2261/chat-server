using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//여기서 룸 센딩을 처리하면된다.
//dictionary 가지고있어야함.<int,Room>
namespace SyncServer
{
    class RoomManager
    {
        private static object _lock = new object();

        private static Dictionary<int, Room> chatroomList = new Dictionary<int, Room>();

        //Room객체에 최대접속인원 필드를 넣을거면, 최대인원 수용 불가능=-1 리턴
        public static int Enter(ClientSession cSession, byte[] bodyData, Socket socket)
        {
            bool maxCheck = true;
            if (maxCheck)
            {
                int rNumCand = BitConverter.ToInt16(bodyData);
                lock (_lock)
                {
                    if (chatroomList.ContainsKey(rNumCand))
                        chatroomList[rNumCand].AddMember(rNumCand, socket,cSession.id);
                    else
                    {
                        Room newRoom = new Room();
                        newRoom.AddMember(rNumCand, socket,cSession.id);
                        chatroomList.Add(rNumCand, newRoom);
                    }
                }
                
                short others = (short) PACKETID.NTF_ROOM_NEW_USER;
                byte[] entermsg = Encoding.UTF8.GetBytes($"{cSession.id} 님이 {rNumCand}방에 입장하셨습니다.");
                short listbox = (short) PACKETID.NTF_ROOM_USER_LIST;
                byte[] enterid = Encoding.UTF8.GetBytes(cSession.id);
                short personal = 1016;
                byte[] msgtoMe = Encoding.UTF8.GetBytes($"{rNumCand}방에 입장하셨습니다.");
                //broadcast to other member
                SendManager.BroadCast(chatroomList[rNumCand].RoomMember, socket, others, entermsg, 1);
                //update list
                SendManager.BroadCast(chatroomList[rNumCand].RoomMember, socket, listbox, enterid, 1);
                //broadcast to me
                SendManager.BroadCast(socket, personal, msgtoMe);
                //Sending member list to me
                foreach (string memberstr in chatroomList[rNumCand].MemberIds)
                {
                    byte[] member =  Encoding.UTF8.GetBytes(memberstr);
                    SendManager.BroadCast(socket,listbox,member);
                }
                return rNumCand;
            }
            else
                return -1;
        }

        public static void Leave(ClientSession cSession, byte[] ?bodyData, Socket ?socket, int flag)
        {
            if (cSession.roomNum == -1)
            {
                Console.WriteLine("bad");
            }
            else
            {
                int rNum = cSession.roomNum;
                Console.WriteLine(rNum);
                lock (_lock)
                {
                    chatroomList[rNum].DeleteMember(socket, cSession.id);
                }

                //broadcast to others user leave
                short roomchatid = (short) PACKETID.NTF_ROOM_CHAT;
                byte[] leaveMsg = Encoding.UTF8.GetBytes($"{cSession.id} 님이 퇴장하셨습니다.");
                SendManager.BroadCast(chatroomList[rNum].RoomMember, socket, roomchatid, leaveMsg, 0);
                //update room member list (to 기존멤버들에게)
                short listbox = (short) PACKETID.NTF_ROOM_LEAVE_USER;
                byte[] leaveId = Encoding.UTF8.GetBytes(cSession.id);
                SendManager.BroadCast(chatroomList[rNum].RoomMember, socket, listbox, leaveId, 0);
                //state update to me
                if (flag == 0)
                {
                    short leaveMe = (short) PACKETID.RES_ROOM_LEAVE;
                    SendManager.BroadCast(socket, leaveMe, leaveId);
                }

            }
  

        }

        public static void Chat(ClientSession cSession, byte[] bodyData, Socket socket)
        {
            //broadcast to other message
            short roomchatid = (short) PACKETID.NTF_ROOM_CHAT;
            string msg =  $"[{cSession.id}] : {Encoding.UTF8.GetString(bodyData)}";
            byte[] msgByte = Encoding.UTF8.GetBytes(msg);
            //Broadcast(보낼소켓,현재소켓,packetid(header),body,패킷전달에 현재소켓 포함여부)
            SendManager.BroadCast(chatroomList[cSession.roomNum].RoomMember, socket , roomchatid,msgByte,0);

        }
    }
}