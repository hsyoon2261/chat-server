using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
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
            try
            {
                

            }
            catch (Exception e)
            {
                labelConnState.Content = string.Format("{0}. 서버에 접속 실패", e.ToString());
            }

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