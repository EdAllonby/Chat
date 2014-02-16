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
            serverData.RegisterObserver(this);
        }

        public void Update(Client client)
        {
            clientUpdate = client;
            Log();
        }

        public void Log()
        {
            Console.WriteLine("Client ID: {0}, Client Message: {1}, Client Status: {2}", clientUpdate.GetUserId(),
                clientUpdate.GetMessage(), clientUpdate.GetStatus());
        }
    }
}