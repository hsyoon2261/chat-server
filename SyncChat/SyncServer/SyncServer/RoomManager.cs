﻿using System;
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
        private static Dictionary<int, Room> chatroomList = new Dictionary<int, Room>();
        //Room객체에 최대접속인원 필드를 넣을거면, 최대인원 수용 불가능=-1 리턴
        public static int Enter(ClientSession cSession, byte[] bodyData, Socket socket)
        {
            bool maxCheck = true;
            if (maxCheck)
            {
                int rNumCand = BitConverter.ToInt16(bodyData);
                if (chatroomList.ContainsKey(rNumCand))
                    chatroomList[rNumCand].AddMember(rNumCand, socket);
                else
                {
                    Room newRoom = new Room();
                    newRoom.AddMember(rNumCand, socket);
                    chatroomList.Add(rNumCand, newRoom);
                }

                short others = 1015;
                byte[] sendothers = Encoding.UTF8.GetBytes($"{cSession.id} 님이 {rNumCand}방에 입장하셨습니다.");

                short personal = 1016;
                byte[] toMe = Encoding.UTF8.GetBytes($"{rNumCand}방에 입장하셨습니다.");
                //broadcast to other member
                SendManager.BroadCast(chatroomList[rNumCand].RoomMember ,socket ,others ,sendothers);
                //broadcast to me
                SendManager.BroadCast(socket,personal,toMe);
                return rNumCand;
            }
            else
                return -1;
        }

        public static void Leave(ClientSession cSession, byte[] bodyData, Socket socket)
        {
            throw new NotImplementedException();
        }

        public static void Chat(ClientSession cSession, byte[] bodyData, Socket socket)
        {
            throw new NotImplementedException();
        }
    }
}
