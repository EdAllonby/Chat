using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ServerData : ISubject
    {
        private readonly List<IObserver> observers;
        private Client clientData;

        private int portNumber = 5004;

        public ServerData()
        {
            observers = new List<IObserver>();
        }

        public void RegisterObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update(clientData);
            }
        }

        public void ClientUpdate()
        {
            NotifyObservers();
        }

        public void TcpInput(Client client)
        {
            clientData = client;
            ClientUpdate();
        }

        public void TcpServerRun()
        {
            var tcpListener = new TcpListener(IPAddress.Any, portNumber);
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                var tcpHandlerThread = new Thread(TcpHandler);
                tcpHandlerThread.Start(client);
            }
        }

        public void TcpHandler(object client)
        {
            var mClient = (TcpClient) client;
            NetworkStream stream = mClient.GetStream();
            while (true)
            {
                // Read or Write
            }
        }
    }
}