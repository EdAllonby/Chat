using System.Threading;
using SharedClasses.Serialisation;

namespace Server
{
    public static class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static void Main()
        {
            Thread.CurrentThread.Name = "Main Thread";
            Log.Info("Server started");
            StartServer();
        }

        private static void StartServer()
        {
            // Create a a Server object (which will be used eventually to accept TCP connection)
            Log.Info("Set serialise method");
            var serverData = new Server(new BinaryFormat());
        }
    }
}