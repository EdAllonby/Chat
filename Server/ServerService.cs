using System.ServiceProcess;
using System.Threading;
using SharedClasses;
using SharedClasses.Domain;

namespace Server
{
    public partial class ServerService : ServiceBase
    {
        private Server server;

        public ServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var serverThread = new Thread(StartServer) { Name = "Server Thread" };
            serverThread.Start();
        }

        private void StartServer()
        {
            IServiceRegistry serviceRegistry = RegisterServices();
            server = new Server(serviceRegistry);
        }

        protected override void OnStop()
        {
            if (server != null)
            {
                server.Shutdown();
            }
        }

        private static IServiceRegistry RegisterServices()
        {
            IServiceRegistry serviceRegistry = new ServiceRegistry();

            var repositoryManager = new RepositoryManager();
            repositoryManager.AddRepository<User>(new UserRepository());
            repositoryManager.AddRepository<Conversation>(new ConversationRepository());
            repositoryManager.AddRepository<Participation>(new ParticipationRepository());

            serviceRegistry.RegisterService<RepositoryManager>(repositoryManager);
            serviceRegistry.RegisterService<IClientManager>(new ClientManager());
            serviceRegistry.RegisterService<EntityIdAllocatorFactory>(new EntityIdAllocatorFactory());

            return serviceRegistry;
        }
    }
}