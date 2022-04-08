using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WpfApp1
{
    public class TcpController
    {
        public Socket Sock = null;   
        public string LatestErrorMsg;
        

        //소켓연결        
        public bool Connect(string ip, int port)
        {
            //string host = Dns.GetHostName();
            //IPHostEntry ipHost = Dns.GetHostEntry(host);
            //IPAddress ipAddr = ipHost.AddressList[1];
            try
            {
                IPAddress serverIP = IPAddress.Parse(ip);
                int serverPort = port;

                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Sock.Connect(new IPEndPoint(serverIP, serverPort));
                if (Sock.Connected == true)
                {
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World this is for test");
                    int sendBytes = Sock.Send(sendBuff);
                }


                if (Sock == null || Sock.Connected == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LatestErrorMsg = ex.Message;
                return false;
            }
        }

        public Tuple<int,byte[]> Receive()
        {

            try
            {
                byte[] ReadBuffer = new byte[2048];
                var nRecv = Sock.Receive(ReadBuffer, 0, ReadBuffer.Length, SocketFlags.None);

                if (nRecv == 0)
                {
                    return null;
                }

                return Tuple.Create(nRecv,ReadBuffer);
            }
            catch (SocketException se)
            {
                LatestErrorMsg = se.Message;
            }

            return null;
        }

        //스트림에 쓰기
        public void Send(byte[] sendData)
        {
            try
            {
                if (Sock != null && Sock.Connected) //연결상태 유무 확인
                {
                    Sock.Send(sendData, 0, sendData.Length, SocketFlags.None);
                }
                else
                {
                    LatestErrorMsg = "먼저 채팅서버에 접속하세요!";
                }
            }
            catch (SocketException se)
            {
                LatestErrorMsg = se.Message;
            }
        }

        //소켓과 스트림 닫기
        public void Close()
        {
            if (Sock != null && Sock.Connected)
            {
                //Sock.Shutdown(SocketShutdown.Both);
                Sock.Close();
            }
        }

        public bool IsConnected() { return (Sock != null && Sock.Connected) ? true : false; }
    }


}
