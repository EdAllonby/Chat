using System;
using System.IO;
using System.Threading;
using System.Windows;
using log4net;
using log4net.Config;

namespace ChatClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (App));

        /// <summary>
        ///     Method used to get the command line arguments of the project.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            ConfigureAndWatch();
#else
            ConfigureAndLockChanges();
#endif

            Console.Title = "Client";

            Thread.CurrentThread.Name = "Main Thread";
            LoginWindow.CommandLineArguments = e.Args;
            base.OnStartup(e);
        }

#if DEBUG

        private static void ConfigureAndWatch()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
            Log.Debug("Log4Net set to debug mode. All messages are logged");
        }

#else

private static void ConfigureAndLockChanges()
    {
        // Disable using DEBUG mode in Release mode (to make harder to hack)
        XmlConfigurator.Configure(new FileInfo("log4net.config"));
        foreach (ILoggerRepository repository in LogManager.GetAllRepositories())
        {
            repository.Threshold = Level.Warn;
        }
    }

#endif
    }
}