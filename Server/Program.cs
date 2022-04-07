using System;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

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