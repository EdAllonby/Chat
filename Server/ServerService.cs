using System.ServiceProcess;
using System.Threading;

namespace Server
{
    public partial class ServerService : ServiceBase
    {
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
            var Server = new Server();
        }

        protected override void OnStop()
        {
        }
    }
}