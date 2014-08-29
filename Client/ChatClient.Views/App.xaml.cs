using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ChatClient.Services;
using log4net;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (App));

        protected override void OnStartup(StartupEventArgs e)
        {

#if DEBUG
            // Console must be started before configuring log4net.
            ConsoleManager.Show();
            SetupLogging("log4netDebug.config");
#else
            SetupLogging("log4netRelease.config");
#endif
            Thread.CurrentThread.Name = "Main Thread";

            IServiceRegistry serviceRegistry = CreateLoadedServiceRegistry();
            
            var loginWindow = new LoginWindow(serviceRegistry);
            loginWindow.Show();

            base.OnStartup(e);
        }

        private static void SetupLogging(string logConfigName)
        {
            string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(App)).Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            if (assemblyDirectory != null)
            {
                var uri = new Uri(Path.Combine(assemblyDirectory, logConfigName));

                log4net.Config.XmlConfigurator.Configure(uri);
            }
        }

        private static IServiceRegistry CreateLoadedServiceRegistry()
        {
            IServiceRegistry serviceRegistry = new ServiceRegistry();

            var repositoryManager = new RepositoryManager();

            repositoryManager.AddRepository<User>(new UserRepository());
            repositoryManager.AddRepository<Conversation>(new ConversationRepository());
            repositoryManager.AddRepository<Participation>(new ParticipationRepository());

            serviceRegistry.RegisterService<RepositoryManager>(repositoryManager);
            serviceRegistry.RegisterService<IClientService>(new ClientService(serviceRegistry));

            return serviceRegistry;
        }

        protected override void OnExit(ExitEventArgs e)
        {
#if DEBUG
            ConsoleManager.Hide();
#endif
            base.OnExit(e);
        }
    }
}