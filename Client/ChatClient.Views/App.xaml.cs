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
            ConsoleManager.Show();
#endif
            Thread.CurrentThread.Name = "Main Thread";

            IServiceRegistry serviceRegistry = CreateLoadedServiceRegistry();

            Thread.Sleep(1000);
            Log.Debug("Logging in debug mode.");

            base.OnStartup(e);

            var loginWindow = new LoginWindow(serviceRegistry);
            loginWindow.Show();
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