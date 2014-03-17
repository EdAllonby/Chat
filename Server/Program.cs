using System;
using System.Threading;
using log4net;

namespace Server
{
    public static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        private static void Main()
        {
            Thread.CurrentThread.Name = "Main Thread";
            Log.Info("Server started");
            StartServer();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void StartServer()
        {
            // Create a a Server object (which will be used eventually to accept TCP connection)
            var serverData = new Server();
        }
    }
}