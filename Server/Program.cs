using System;
using System.Threading;

namespace Server
{
    public static class Program
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(typeof (Program));


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