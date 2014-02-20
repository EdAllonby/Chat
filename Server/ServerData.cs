using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Server.Serialisation;

namespace Server
{
    public class ServerData : ISubject
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static TcpListener listener;

        private const int ConcurrentSockets = 5;

        private readonly List<IObserver> observers;
        private Client clientData;

        // Use Strategy pattern to chose what TCP Serialisation method to use
        private ITcpSendBehaviour sendBehaviour;

        public const int PortNumber = 5004;

        public ServerData(ITcpSendBehaviour sendBehaviour)
        {
            SetSerialiseMethod(sendBehaviour);

            observers = new List<IObserver>();

            StartTcpInput();
        }

        public void SetSerialiseMethod(ITcpSendBehaviour wantedSendBehaviour)
        {
            sendBehaviour = wantedSendBehaviour;
        }

        public void RegisterObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyObserversOfClientChange()
        {
            foreach (var observer in observers)
            {
                if (clientData != null)
                {
                    observer.Update(clientData);
                }
            }
        }

        private void StartTcpInput()
        {
            listener = new TcpListener(PortNumber);
            listener.Start();

            for (int i = 0; i < ConcurrentSockets; i++)
            {
                var tcpInstance = new Thread(ListenForIncomingData);
                tcpInstance.Start();
            }
        }

        private void ListenForIncomingData()
        {
            while (true)
            {
                Socket socket = listener.AcceptSocket();

                Stream networkStream = new NetworkStream(socket);

                clientData = sendBehaviour.Deserialise(networkStream);

                NotifyObserversOfClientChange();
            }
        }
    }
}