using System.ServiceProcess;
using System.Threading;
using SharedClasses;

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
            var serverThread = new Thread(StartServer) {Name = "Server Thread"};
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

        private IServiceRegistry RegisterServices()
        {
            IServiceRegistry serviceRegistry = new ServiceRegistry();
            serviceRegistry.RegisterService<RepositoryManager>(new RepositoryManager());
            serviceRegistry.RegisterService<IClientManager>(new ClientManager());
            serviceRegistry.RegisterService<EntityIdAllocatorFactory>(new EntityIdAllocatorFactory());

            return serviceRegistry;
        }
    }
}