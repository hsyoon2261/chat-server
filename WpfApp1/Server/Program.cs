using System;
using System.Net;
using Core;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IPInformation ipInfo = new IPInformation(Dns.GetHostName(), 12345);

            Listener listener = new Listener();
            
            listener.Init(ipInfo.EndPoint, ()=> new ChattingClientSession());

            while (true) ;
        }
    }
}