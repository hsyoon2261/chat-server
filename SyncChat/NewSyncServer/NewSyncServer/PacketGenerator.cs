using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NewSyncServer
{
    public struct PacketData
    {
        public Int16 DataSize;
        public Int16 PacketID;
        public SByte Type;
        public byte[] BodyData;
    }

    public class PacketProcess
    {
        private int PacketHeaderSize = 5;
        
        public void Init(ClientSession clientSession, int recvBytes, byte[] recvBuff)
                {
                    var readpacket = new byte[recvBytes];
                    Buffer.BlockCopy(recvBuff, 0, readpacket, 0, recvBytes);
                    Queue<PacketData> RecvPacketQueue = CuttingPacket(recvBytes,readpacket);
                            
                    while (RecvPacketQueue.Count > 0)
                    {
                        var sendpacket = RecvPacketQueue.Dequeue();
                        Send( clientSession,sendpacket);
                    }
                }

        public Queue<PacketData> CuttingPacket(int recvBytes, byte[] readPacket)
        {
            Queue<PacketData> packetQueue = new Queue<PacketData>();
            var dataSize = readPacket.Length;
            var startPoint = 0;
            while (startPoint < dataSize)
            {
                int packetsize = BitConverter.ToInt16(readPacket, startPoint);
                var packet = new PacketData();
                packet.DataSize = (short) (readPacket.Length - PacketHeaderSize);
                packet.PacketID = BitConverter.ToInt16(readPacket, startPoint + 2);
                packet.Type = (sbyte) readPacket[startPoint + 4];
                packet.BodyData = new byte[packet.DataSize];
                Buffer.BlockCopy(readPacket, PacketHeaderSize, packet.BodyData, 0, packet.DataSize);
                startPoint += packetsize;
                packetQueue.Enqueue(packet);
            }

            return packetQueue;
        }

        public void Send(ClientSession user, PacketData sendPacket)
        {
            switch (sendPacket.PacketID)
            {
                case (short) PACKETID.RES_LOGIN:
                    if (user.mode == ClientSession.Mode.Lobby)
                    {
                        string id = Encoding.UTF8.GetString(sendPacket.BodyData);
                        user.GetName(id);
                        GameUser.Insert(user,ClientSession.Mode.Lobby);
                        GameUser.Insert(user,ClientSession.Mode.Login);
                        user.mode = ClientSession.Mode.Login;
                        var welcomeMsg = $"{id}님 로그인 성공하셨습니다.";
                        var sendMsg = PacketToBytes((short) PACKETID.RES_LOGIN, Encoding.UTF8.GetBytes(welcomeMsg));
                        user._socket.Send(sendMsg);
                    }
                    break;
            }
        }
        
        public byte[] PacketToBytes(Int16 packetID, byte[] bodyData)
        {
            byte type = 0;
            var pktID = (Int16) packetID;
            Int16 bodyDataSize = 0;
            if (bodyData != null)
            {
                bodyDataSize = (Int16) bodyData.Length;
            }

            var packetSize = (Int16) (bodyDataSize + PacketHeaderSize);

            var dataSource = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(pktID), 0, dataSource, 2, 2);
            dataSource[4] = type;

            if (bodyData != null)
            {
                Buffer.BlockCopy(bodyData, 0, dataSource, 5, bodyDataSize);
            }

            return dataSource;
        }

        
    }
}