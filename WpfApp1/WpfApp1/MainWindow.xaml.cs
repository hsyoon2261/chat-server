using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Core;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum CLIENT_STATE
        {
            NONE = 0,
            CONNECTED = 1,
            LOGIN = 2,
            ROOM = 3
        }
        CLIENT_STATE ClientState = CLIENT_STATE.NONE;
        bool IsBackGroundProcessRunning = false;
        System.Windows.Threading.DispatcherTimer dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();



        
        public MainWindow()
        {
            InitializeComponent();
            IsBackGroundProcessRunning = true;

            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherUITimer.Start();
            if (ClientState == CLIENT_STATE.CONNECTED)
            {
                labelConnState.Content = string.Format(" 서버에 접속 실패");

            }




        }
        //가입
        private void Button_Click_0(object sender, RoutedEventArgs e)
        {
            labelConnState.Content = string.Format("먼저 접속해 주십시오.");
        }


        //접속
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string address = textBoxIP.Text;
            if (checkBoxLocalHostIP.IsChecked == true)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt16(textBoxPort.Text);
            IPAddress ipAd = IPAddress.Parse(address);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAd, port);
            Socket socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);

            // Connector connector = new Connector();
            // connector.Connect(ipEndPoint,()=> new ChattingServerSession());
            // ClientState = CLIENT_STATE.CONNECTED;
            

            // try
            // {
            //                     
            //
            // }
            // catch (Exception er)
            // {
            //     labelConnState.Content = string.Format("{0}. 서버에 접속 실패", er.ToString());
            // }

        }

        // 로그아웃
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
        }

        // 로그인
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        // 접속 끊기
        void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }

        // 방 입장
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
        }

        // 방 나가기
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
        }

        // 방 채팅
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
        }

        //버튼을 추가하였습니다.
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
          
        }

        // 모든 게임 방 게임 시작
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
         
        }

        // 모든 게임 방 게임 끝
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
          
        }
    }
}