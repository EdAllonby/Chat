using System;
using Server.Serialisation;

namespace Server
{
    public static class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main()
        {
            StartServer();

            //You should try to call the logger as soon as possible in your application
            log.Debug("Application started");
        }

        private static void StartServer()
        {
            // Create a a ServerData object (which will be used eventually to accept TCP connection)
            var serverData = new ServerData(new BinaryFormat());

            log.Debug("Debug logging");
            log.Info("Info logging");
            log.Warn("Warn logging");
            log.Error("Error logging");
            log.Fatal("Fatal logging");

            //This is where we call the logger from a different class
            OtherClass.TestLogger();
            Console.ReadKey();
        }
    }

    //Designed to show how we can change log levels by class or namespace
    public static class OtherClass
    {
        //Here is the once-per-class call to initialize the log object
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Used to demonstrate making log calls from a different namespace
        public static void TestLogger()
        {
            log4net.GlobalContext.Properties["testProperty"] = "This is my test property information";

            log.Debug("Other Class - Debug logging");
            log.Info("Other Class - Info logging");
            log.Warn("Other Class - Warn logging");
            log.Error("Other Class - Error logging");
            log.Fatal("Other Class - Fatal logging");
        }
    }
}