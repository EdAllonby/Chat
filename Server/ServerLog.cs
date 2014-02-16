using System;

namespace Server
{
    public class ServerLog : IObserver, ILogger
    {
        private Client clientUpdate;
        
        private ISubject serverData;

        public ServerLog(ISubject serverData)
        {
            this.serverData = serverData;
        }

        public void Update(Client client)
        {
            clientUpdate = client;
            Log();
        }

        public void Log()
        {
            Console.WriteLine("Client ID: {0}");
        }
    }
}