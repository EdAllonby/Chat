using System;

namespace Server
{
    public class ServerLog : IObserver
    {
        private Client clientUpdate;

        private ISubject serverData;

        public ServerLog(ISubject serverData)
        {
            Console.WriteLine("Server log started, connected to: {0}", serverData);
            Console.WriteLine();

            this.serverData = serverData;
            serverData.RegisterObserver(this);
        }

        public void Update(Client client)
        {
            clientUpdate = client;
            Log();
        }

        private void Log()
        {
            if (!string.IsNullOrEmpty(clientUpdate.GetMessage()))
            {
                Console.WriteLine("Client ID: {0}, Client Status: {1} Client Message: {2}", clientUpdate.GetUserId(),
                    clientUpdate.GetStatus(), clientUpdate.GetMessage());
            }
            else
            {
                Console.WriteLine("Client ID: {0} has {1}", clientUpdate.GetUserId(),
                    clientUpdate.GetStatus());
            }
        }
    }
}