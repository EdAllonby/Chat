using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using log4net;

namespace Server
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            Thread mainThread = Thread.CurrentThread;
            mainThread.Name = "Main Thread";

            var servicesToRun = new ServiceBase[]
            {
                new ServerService()
            };
            if (Environment.UserInteractive)
            {
                RunInteractive(servicesToRun);
            }
            ServiceBase.Run(servicesToRun);
        }

        private static void RunInteractive(ServiceBase[] servicesToRun)
        {
            Log.Debug("Services running in interactive mode.");

            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Log.DebugFormat($"Starting {service.ServiceName}...");
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Log.Debug("Started");
            }

            Log.Debug("Press any key to stop the services and end the process...");
            Console.ReadKey();
            Console.WriteLine();

            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Log.DebugFormat($"Stopping {service.ServiceName}...");
                onStopMethod.Invoke(service, null);
                Log.Info("Server Stopped");
            }

            Log.Info("All services stopped.");
            // Keep the console alive for a second to allow the user to see the message.
            Thread.Sleep(1000);
        }
    }
}