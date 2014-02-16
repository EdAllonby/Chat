using System.Collections.Generic;

namespace Server
{
    public class ServerData : ISubject
    {
        private readonly List<IObserver> observers;
        private Client clientData;

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
    }
}