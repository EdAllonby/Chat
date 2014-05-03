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

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            var serverThread = new Thread(StartServer);
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