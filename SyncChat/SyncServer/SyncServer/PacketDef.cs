using System;

namespace SyncServer
{
    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;

        public const int INVALID_ROOM_NUMBER = -1;
    }

    public class PacketToBytes
    {
        public static byte[] Make(Int16 packetID, byte[] bodyData)
        {
            byte type = 0;
            var pktID = (Int16) packetID;
            Int16 bodyDataSize = 0;
            if (bodyData != null)
            {
                bodyDataSize = (Int16) bodyData.Length;
            }

            var packetSize = (Int16) (bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

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