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
            StartServer();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void StartServer()
        {
            Log.Info("Starting server instance");
            var serverData = new Server();
        }
    }
}