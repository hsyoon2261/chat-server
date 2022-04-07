using System;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //server setting
            var serverOption = ParseCommandLine(args);
            if(serverOption == null)
            {
                //use default setting
                return;
            }

           //Mainserver = socket 통신 진입 class
            var serverApp = new MainServer();
            serverApp.InitConfig(serverOption);

            serverApp.CreateStartServer();

            MainServer.MainLogger.Info("Press q to shut down the server");
            
            while (true)
            {
                //why?
                System.Threading.Thread.Sleep(50);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.KeyChar == 'q')
                    {
                        Console.WriteLine("Server Terminate ~~~");
                        serverApp.StopServer();
                        break;
                    }
                }
                                
            }

        }
        //ChatServerOption세팅, 이걸 활용해서 port, max client number등 변경시에
        //재빌드없이 command로 쉽게 처리 가능.
        static ChatServerOption ParseCommandLine(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<ChatServerOption>(args) as CommandLine.Parsed<ChatServerOption>;

            if (result == null)
            {
                Console.WriteLine("Failed Command Line");
                return null;
            }

            return result.Value;
        }
    }
    

}