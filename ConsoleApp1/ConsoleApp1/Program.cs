using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 1026;
            var abc = BitConverter.GetBytes(a);
            Console.WriteLine($"{abc}");
            Console.WriteLine("Hello World!");
        }
    }
}