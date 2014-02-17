using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Server.Serialisation;

namespace Server
{
    public class ServerData : ISubject
    {
        private static TcpListener listener;

        private const int Limit = 5; //Max 5 concurrent clients

        private readonly List<IObserver> observers;
        private Client clientData;

        // Use Strategy pattern to chose what TCP Serialisation method to use
        private ITcpSendBehaviour sendBehaviour;

        public const int portNumber = 5004;

        public ServerData(ITcpSendBehaviour sendBehaviour)
        {
            this.sendBehaviour = sendBehaviour;
            observers = new List<IObserver>();

            // Start TCP Listen
            TcpInput();
        }

        public void SetSerialiseMethod(ITcpSendBehaviour sendBehaviour)
        {
            this.sendBehaviour = sendBehaviour;
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

        public void TcpInput()
        {
            listener = new TcpListener(portNumber);
            listener.Start();

            for (int i = 0; i < Limit; i++)
            {
                var t = new Thread(Service);
                t.Start();
            }
        }

        private void Service()
        {
            while (true)
            {
                var socket = listener.AcceptSocket();

                Stream networkStream = new NetworkStream(socket);
                clientData = sendBehaviour.Deserialise(networkStream);

                if (clientData != null)
                {
                    ClientUpdate();
                }
            }
        }
    }
}