using CSBaseLib;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        CLIENT_STATE ClientState = CLIENT_STATE.NONE;

        ClientSimpleTcp Network = new ClientSimpleTcp();

        bool IsNetworkThreadRunning = false;
        bool IsBackGroundProcessRunning = false;

        System.Threading.Thread NetworkReadThread = null;
        System.Threading.Thread NetworkSendThread = null;

        PacketBufferManager PacketBuffer = new PacketBufferManager();
        Queue<PacketData> RecvPacketQueue = new Queue<PacketData>();
        Queue<byte[]> SendPacketQueue = new Queue<byte[]>();

        System.Windows.Threading.DispatcherTimer dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();

        private List<string> roomMember = new List<string>();
        private string userName;

        enum CLIENT_STATE
        {
            NONE = 0,
            CONNECTED = 1,
            LOGIN = 2,
            ROOM = 3
        }

        struct PacketData
        {
            public Int16 DataSize;
            public Int16 PacketID;
            public SByte Type;
            public byte[] BodyData;
        }

        public MainWindow()
        {
            InitializeComponent();

            PacketBuffer.Init((8096 * 10), 5, 1024);

            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            NetworkSendThread.Start();

            IsBackGroundProcessRunning = true;
            dispatcherUITimer.Tick += new EventHandler(BackGroundProcess);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherUITimer.Start();
        }

        void BackGroundProcess(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                var packet = new PacketData();

                lock (((System.Collections.ICollection) RecvPacketQueue).SyncRoot)
                {
                    if (RecvPacketQueue.Count() > 0)
                    {
                        packet = RecvPacketQueue.Dequeue();
                    }
                }

                if (packet.PacketID != 0)
                {
                    PacketProcess(packet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ReadPacketQueueProcess. error:{0}", ex.Message));
            }
        }

        private void ProcessLog()
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (IsBackGroundProcessRunning)
            {
                System.Threading.Thread.Sleep(1);

                string msg;

                if (DevLog.GetLog(out msg))
                {
                    ++logWorkCount;

                    if (listBoxLog.Items.Count > 512)
                    {
                        listBoxLog.Items.Clear();
                    }

                    listBoxLog.Items.Add(msg);
                    var lastIndex = listBoxLog.Items.Count - 1;
                    listBoxLog.ScrollIntoView(listBoxLog.Items[lastIndex]);
                }
                else
                {
                    break;
                }

                if (logWorkCount > 8)
                {
                    break;
                }
            }
        }

        void NetworkReadProcess()
        {
            const Int16 PacketHeaderSize = CSBaseLib.PacketDef.PACKET_HEADER_SIZE;

            while (IsNetworkThreadRunning)
            {
                if (Network.IsConnected() == false)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                var recvData = Network.Receive();

                if (recvData != null)
                {
                    PacketBuffer.Write(recvData.Item2, 0, recvData.Item1);

                    while (true)
                    {
                        var data = PacketBuffer.Read();
                        if (data.Count < 1)
                        {
                            break;
                        }

                        var packet = new PacketData();
                        packet.DataSize = (short) (data.Count - 5);
                        packet.PacketID = BitConverter.ToInt16(data.Array, data.Offset + 2);
                        packet.Type = (SByte) data.Array[(data.Offset + 4)];
                        packet.BodyData = new byte[packet.DataSize];
                        Buffer.BlockCopy(data.Array, (data.Offset + 5), packet.BodyData, 0,
                            (data.Count - 5));
                        lock (((System.Collections.ICollection) RecvPacketQueue).SyncRoot)
                        {
                            RecvPacketQueue.Enqueue(packet);
                        }
                    }
                    //DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
                }
                else
                {
                    SetDisconnectd();
                    Thread.Sleep(100);
                    Network.Close();
                    DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
                }
            }
        }

        void NetworkSendProcess()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(1);

                if (Network.IsConnected() == false)
                {
                    continue;
                }

                lock (((System.Collections.ICollection) SendPacketQueue).SyncRoot)
                {
                    if (SendPacketQueue.Count > 0)
                    {
                        var packet = SendPacketQueue.Dequeue();
                        Network.Send(packet);
                    }
                }
            }
        }

        //TODO
        public void SetDisconnectd()
        {
            var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.NTF_MUST_CLOSE, null);
            PostSendPacket(sendData);
            ClientState = CLIENT_STATE.NONE;
            listBoxRoomUserList.Items.Clear();


            //SendPacketQueue.Clear();

            //ClearUIRoomOut();
            //labelStatus.Text = "서버 접속이 끊어짐";
        }

        void ClearUIRoomOut()
        {
            listBoxRoomUserList.Items.Clear();
            textBoxRoomNum.Text = "-1";
            listBoxChat.Items.Clear();
        }

        void RequestEcho(string message)
        {
            var body = message.ToByteArray();

            DevLog.Write($"서버에 Echo 요청. BodySize:{body.Length}", LOG_LEVEL.INFO);

            var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_RES_TEST_ECHO, body);
            PostSendPacket(sendData);
        }

        void RequestLogin(string userID, string authToken)
        {
            DevLog.Write("서버에 로그인 요청", LOG_LEVEL.INFO);

            var reqLogin = new CSBaseLib.PKTReqLogin() {UserID = userID, AuthToken = authToken};
            byte[] Body2 = Encoding.UTF8.GetBytes(userID);
            //var Body = MessagePackSerializer.Serialize(reqLogin);
            var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.RES_LOGIN, Body2);
            PostSendPacket(sendData);
        }

        void RequestRoomEnter(int roomnumber)
        {
            byte[] body = BitConverter.GetBytes(roomnumber);
            var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_ENTER, body);
            PostSendPacket(sendData);
        }

        public void PostSendPacket(byte[] sendData)
        {
            if (Network.IsConnected() == false)
            {
                DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
                return;
            }

            SendPacketQueue.Enqueue(sendData);
        }


        void PacketProcess(PacketData packet)
        {
            switch ((PACKETID) packet.PacketID)
            {
                case PACKETID.REQ_RES_TEST_ECHO:
                {
                    DevLog.Write($"Echo 응답: {packet.BodyData.Length}", LOG_LEVEL.INFO);
                    break;
                }
                case PACKETID.RES_LOGIN:
                {
                    ClientState = CLIENT_STATE.LOGIN;
                    userName = Encoding.UTF8.GetString(packet.BodyData);
                    DevLog.Write($"로그인 성공, {userName}", LOG_LEVEL.INFO);
                }
                    break;

                case PACKETID.RES_ROOM_ENTER:
                {
                    ClientState = CLIENT_STATE.ROOM;
                    DevLog.Write("방 입장 성공", LOG_LEVEL.INFO);
                    roomMember.Add(userName);
                    // var resData = MessagePackSerializer.Deserialize<PKTResRoomEnter>(packet.BodyData);
                    //
                    // if (resData.Result == (short) ERROR_CODE.NONE)
                    // {
                    //     ClientState = CLIENT_STATE.ROOM;
                    //     DevLog.Write("방 입장 성공", LOG_LEVEL.INFO);
                    // }
                    // else
                    // {
                    //     DevLog.Write(
                    //         string.Format("방입장 실패: {0} {1}", resData.Result, ((ERROR_CODE) resData.Result).ToString()),
                    //         LOG_LEVEL.INFO);
                    // }
                }
                    break;
                case PACKETID.NTF_ROOM_USER_LIST:
                {
                    listBoxRoomUserList.Items.Add(Encoding.UTF8.GetString(packet.BodyData));

                    //var ntfData = MessagePackSerializer.Deserialize<PKTNtfRoomUserList>(packet.BodyData);

                    // foreach (var user in ntfData.UserIDList)
                    // {
                    //     listBoxRoomUserList.Items.Add(user);
                    // }
                }
                    break;
                case PACKETID.NTF_ROOM_NEW_USER:
                {
                    string notice = Encoding.UTF8.GetString(packet.BodyData);
                    listBoxChat.Items.Add(notice);
                }
                    break;

                case PACKETID.RES_ROOM_LEAVE:
                {
                    listBoxRoomUserList.Items.Clear();
                    ClientState = CLIENT_STATE.LOGIN;
                    DevLog.Write("방 나가기 성공", LOG_LEVEL.INFO);

                    // }
                    // else
                    // {
                    //     DevLog.Write(
                    //         string.Format("방 나가기 실패: {0} {1}", resData.Result,
                    //             ((ERROR_CODE) resData.Result).ToString()), LOG_LEVEL.ERROR);
                    // }
                }
                    break;
                case PACKETID.NTF_ROOM_LEAVE_USER:
                {
                    string leaveid = Encoding.UTF8.GetString(packet.BodyData);
                    //var ntfData = MessagePackSerializer.Deserialize<PKTNtfRoomLeaveUser>(packet.BodyData);
                    listBoxRoomUserList.Items.Remove(leaveid);
                }
                    break;

                case PACKETID.NTF_ROOM_CHAT:
                {
                    textBoxSendChat.Text = "";
                    string chatmsg = Encoding.UTF8.GetString(packet.BodyData);
                    listBoxChat.Items.Add($"{chatmsg}");
                    //var ntfData = MessagePackSerializer.Deserialize<PKTNtfRoomChat>(packet.BodyData);
                    //listBoxChat.Items.Add($"[{ntfData.UserID}]: {ntfData.ChatMessage}");
                }
                    break;
            }
        }


        // 접속하기
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string address = textBoxIP.Text;

            if (checkBoxLocalHostIP.IsChecked == true)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(textBoxPort.Text);

            DevLog.Write($"서버에 접속 시도: ip:{address}, port:{port}", LOG_LEVEL.INFO);

            if (Network.Connect(address, port))
            {
                labelConnState.Content = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                ClientState = CLIENT_STATE.CONNECTED;
            }
            else
            {
                labelConnState.Content = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }
        }


        // Echo
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            RequestEcho(textBoxEcho.Text);
        }

        // 로그인
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ClientState == CLIENT_STATE.CONNECTED)
            {
                RequestLogin(textBoxID.Text, textBoxPW.Text);
            }
            else
            {
                DevLog.Write("연결상태 아님. 접속을 먼저 하십시오.");
            }
        }

        // 접속 끊기
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DevLog.Write($"서버 접속 끊기", LOG_LEVEL.INFO);

            //ClientState = CLIENT_STATE.NONE;
            SetDisconnectd();
        }

        // 방 입장
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            switch (ClientState)
            {
                case CLIENT_STATE.NONE:
                    DevLog.Write("접속 부터 하십시오.", LOG_LEVEL.INFO);
                    break;
                case CLIENT_STATE.ROOM:
                    DevLog.Write("먼저 방을 나가십시오.", LOG_LEVEL.INFO);
                    break;
                case CLIENT_STATE.LOGIN:
                    var roomNum = textBoxRoomNum.Text.ToInt16();
                    DevLog.Write("서버에 방 입장 요청", LOG_LEVEL.INFO);
                    RequestRoomEnter(roomNum);
                    roomMember.Add(userName);
                    break;
                case CLIENT_STATE.CONNECTED:
                    DevLog.Write("로그인 부터 하십시오.", LOG_LEVEL.INFO);
                    break;

            }

            //var request = new CSBaseLib.PKTReqRoomEnter() {RoomNumber = roomNum};

            //var Body = MessagePackSerializer.Serialize(request);
            //var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_ENTER, Body);
            //PostSendPacket(sendData);
        }

        // 방 나가기
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DevLog.Write("서버에 방 나가기 요청", LOG_LEVEL.INFO);
            if(ClientState != CLIENT_STATE.ROOM)
                DevLog.Write($"방에 접속한 상태가 아닙니다. 당신의 상태는 {ClientState.ToString()}입니다.", LOG_LEVEL.INFO);
            else
            {
                var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_LEAVE, null);
                PostSendPacket(sendData);
            }

        }

        // 방 채팅
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //var request = new CSBaseLib.PKTReqRoomChat() {ChatMessage = textBoxSendChat.Text};
            //var Body = MessagePackSerializer.Serialize(request);
            byte[] Body = Encoding.UTF8.GetBytes(textBoxSendChat.Text);
            var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_CHAT, Body);
            PostSendPacket(sendData);
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                byte[] Body = Encoding.UTF8.GetBytes(textBoxSendChat.Text);
                var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_CHAT, Body);
                PostSendPacket(sendData);
                // var request = new CSBaseLib.PKTReqRoomChat() {ChatMessage = textBoxSendChat.Text};
                //
                // var Body = MessagePackSerializer.Serialize(request);
                // var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_CHAT, Body);
                // PostSendPacket(sendData);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SetDisconnectd();
            Thread.Sleep(100);
            Network.Close();

            IsNetworkThreadRunning = false;
            IsBackGroundProcessRunning = false;

            if (NetworkReadThread.IsAlive)
            {
                NetworkReadThread.Join();
            }

            if (NetworkSendThread.IsAlive)
            {
                NetworkSendThread.Join();
            }
        }

        // 모든 게임 방 게임 시작
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_DEV_ALL_ROOM_START_GAME, null);
            // PostSendPacket(sendData);
        }

        // 모든 게임 방 게임 끝
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            // var sendData = CSBaseLib.PacketToBytes.Make(CSBaseLib.PACKETID.REQ_ROOM_DEV_ALL_ROOM_END_GAME, null);
            // PostSendPacket(sendData);
        }
    }
}