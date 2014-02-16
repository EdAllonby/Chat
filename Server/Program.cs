using System;

namespace Server
{
    public static class Program
    {
        static void Main()
        {
            // Create a a ServerData object (which will be used eventually to accept TCP connection)
            var serverData = new ServerData();

            // Create a ServerLog object
            var log = new ServerLog(serverData);

            //Simulate a Client object coming from a socket (After serialisation)
            var newClient = new Client("Ed", null, Status.Connected);
            serverData.TcpInput(newClient);

            Console.ReadKey();
        }
    }
}