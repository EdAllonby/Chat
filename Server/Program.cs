using System.Threading;
using Server.Serialisation;

namespace Server
{
    public static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        private static void Main()
        {
            Thread.CurrentThread.Name = "Main Thread";
            log.Info("Server started");
            StartServer();
        }

        private static void StartServer()
        {
            // Create a a ServerData object (which will be used eventually to accept TCP connection)
            log.Info("Set serialise method");
            var serverData = new ServerData(new BinaryFormat());
        }
    }
}