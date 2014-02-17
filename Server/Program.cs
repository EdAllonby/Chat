using System;
using Server.Serialisation;

namespace Server
{
    public static class Program
    {
        private static void Main()
        {
            RunTestClient();
        }

        private static void RunTestClient()
        {
            // Create a a ServerData object (which will be used eventually to accept TCP connection)
            var serverData = new ServerData(new BinaryFormat());

            // Create a ServerLog object
            var log = new ServerLog(serverData);

            Console.ReadKey();
        }
    }
}