using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace SyncServer
{
    struct PacketData
    {
        public Int16 DataSize;
        public Int16 PacketID;
        public SByte Type;
        public byte[] BodyData;
    }

    public class ClientManager : IDisposable
    {
        private ClientSession cSession;
        private Socket socket { get; set; }
        private string id { get; set; }
        CLIENT_STATE ClientState = CLIENT_STATE.NONE;

        public ClientManager(Socket socket)
        {
            this.socket = socket;
            cSession = new ClientSession(socket);
            ClientState = CLIENT_STATE.CONNECTED;
        }
        
        private bool running = true; //아직안쓰지만 일단 두자.
        private int PacketHeaderSize = 5;
        Queue<PacketData> RecvPacketQueue = new Queue<PacketData>();
        
        
        //TODO Run()Method를 빼내야한다. 상속받아서 override로 빼내자. => 걍 클라정보를 뺌(완료)
        public void Run()
        {
            byte[] recvBuff = new byte[1024];
            try
            {
                while (true)
                {
                    int recvBytes = this.socket.Receive(recvBuff);
                    if (recvBytes != 0)
                    {
                        Console.WriteLine("메시지들어옴");
                        byte[] readpacket = new byte[recvBytes];
                        Buffer.BlockCopy(recvBuff, 0, readpacket, 0, recvBytes);
                        Array.Clear(recvBuff, 0, 1024);
                        int startingPoint = 0;
                        int dataSize = recvBytes;
                        while (startingPoint < dataSize)
                        {
                            int packetsize = BitConverter.ToInt16(readpacket, startingPoint);
                            //TODO 데이터 유실 처리 (완)
                            var data = new ArraySegment<byte>(readpacket, startingPoint, packetsize);
                            var packet = new PacketData();
                            packet.DataSize = (short) (data.Count - PacketHeaderSize);
                            packet.PacketID = BitConverter.ToInt16(data.Array, data.Offset + 2);
                            packet.Type = (SByte) data.Array[(data.Offset + 4)];
                            packet.BodyData = new byte[packet.DataSize];
                            Buffer.BlockCopy(data.Array, (data.Offset + PacketHeaderSize), packet.BodyData, 0,
                                (data.Count - PacketHeaderSize));
                            lock (((System.Collections.ICollection) RecvPacketQueue).SyncRoot)
                            {
                                Thread.Sleep(100); //read오류찾기위해
                                RecvPacketQueue.Enqueue(packet);
                            }

                            startingPoint += packetsize;
                        }

                        string recvData = Encoding.UTF8.GetString(recvBuff, 5, recvBytes - 5);
                        //TODO 패킷 classify, message build , send, readbuffer init switch(Login Logout 
                    }
                    else
                    {
                        Console.WriteLine("소켓을 엑세스하는 동안 오류가 발생했습니다.");
                        break;
                    }

                    while (RecvPacketQueue.Count > 0)
                    {
                        PacketData sendpacket = RecvPacketQueue.Dequeue();
                        ResponseProcess(sendpacket);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("클라이언트쪽에서 접속을 끊었습니다.");
                Dispose();
            }
        }

        void ResponseProcess(PacketData packet)
        {
            if (packet.Type != (sbyte) 0)
                Console.WriteLine("bad");
            else
            {
                //if((ERROR_CODE)packet.PacketID ==null)
                //PacketData sendpacket = PacketGenerator(packet);
                SendProcess(packet);
                Thread.Sleep(300); //send오류 찾기위해
                //byte[] finalsend = PacketToBytes.Make(sendpacket.PacketID, sendpacket.BodyData);
            }
            //update
        }

        void SendProcess(PacketData packet)
        {
            switch (packet.PacketID)
            {
                case 1003: //Login
                    Console.WriteLine("login completed");
                    //this.id
                    cSession.id = Encoding.UTF8.GetString(packet.BodyData);
                    Console.WriteLine($"id : {cSession.id}");
                    SendManager.BroadCast(socket,packet.PacketID,packet.BodyData);
                    ClientState = CLIENT_STATE.LOGIN;
                    break;
                
                case 1015: //RES_ROOM_ENTER 
                    //Todo clientstat != login 인 경우 처리해줘야함.. 언젠가.. 
                    int success = RoomManager.Enter(cSession, packet.BodyData, socket);
                    if (success != -1)
                    {
                        cSession.roomNum = success;
                        ClientState = CLIENT_STATE.ROOM;
                        break;
                    }
                    else
                    //fail message to client
                        break;
                
                case 1021: //RES_ROOM_LEAVE
                    RoomManager.Leave(cSession, packet.BodyData, socket);
                    ClientState = CLIENT_STATE.LOGIN;
                    break;
                case 1026: //response Room Chat + NTF_ROOM_CHAT
                    RoomManager.Chat(cSession, packet.BodyData, socket);
                    break;
                case 1100: //Log out
                    break;
                case 8021: //Disconnect
                    Dispose();
                    break;
            }
        }

        public void Dispose()
        {
            
            socket?.Dispose();
        }

        enum CLIENT_STATE
        {
            NONE = 0,
            CONNECTED = 1,
            LOGIN = 2,
            ROOM = 3
        }
    }
    
}