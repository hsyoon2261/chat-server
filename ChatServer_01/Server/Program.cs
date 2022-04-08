using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        private static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"From Client = {recvData}");

                //send
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to Server");
                clientSocket.Send(sendBuff);
                //kick
                clientSocket.Shutdown(SocketShutdown.Both);
                Console.WriteLine("Bye..");
                while (true)
                {
                    
                }
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[1];
            Console.WriteLine(ipAddr.ToString());
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //start listening from client
            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("listening...");
            while (true)
            {
            }
        }
    }
}


//server setting
     //         var serverOption = ParseCommandLine(args);
     //         if(serverOption == null)
     //         {
     //             //use default setting
     //             return;
     //         }
     //
     //        //Mainserver = socket 통신 진입 class
     //         var serverApp = new MainServer();
     //         serverApp.InitConfig(serverOption);
     //
     //         serverApp.CreateStartServer();
     //
     //         MainServer.MainLogger.Info("Press q to shut down the server");
     //         
     //         while (true)
     //         {
     //             //why?
     //             System.Threading.Thread.Sleep(50);
     //
     //             if (Console.KeyAvailable)
     //             {
     //                 //서버 중단을 막기 위한.. 
     //                 ConsoleKeyInfo key = Console.ReadKey(true);
     //                 if (key.KeyChar == 'q')
     //                 {
     //                     Console.WriteLine("Server Terminate ~~~");
     //                     serverApp.StopServer();
     //                     break;
     //                 }
     //             }
     //                             
     //         }
     //
     //     }
     //     //ChatServerOption세팅, 이걸 활용해서 port, max client number등 변경시에
     //     //재빌드없이 command로 쉽게 처리 가능.
     //     static ChatServerOption ParseCommandLine(string[] args)
     //     {
     //         var result = CommandLine.Parser.Default.ParseArguments<ChatServerOption>(args) as CommandLine.Parsed<ChatServerOption>;
     //
     //         if (result == null)
     //         {
     //             Console.WriteLine("Failed Command Line");
     //             return null;
     //         }
     //
     //         return result.Value;
     //     }
     // }
     //