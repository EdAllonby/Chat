using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ChatClient.Services;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            AllocConsole();
#endif
            Thread.CurrentThread.Name = "Main Thread";

            IServiceRegistry serviceRegistry = CreateLoadedServiceRegistry();

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
            FreeConsole();
#endif
            base.OnExit(e);
        }
    }
}