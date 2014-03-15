using System;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    internal static class Program
    {
        private static TcpClient client;
        private static string clientMessage;

        private static void Main()
        {
            Thread.CurrentThread.Name = "Main Thread";
            var MainClient = new Client();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}