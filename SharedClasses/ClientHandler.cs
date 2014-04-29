using System;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace SharedClasses
{
    /// <summary>
    /// The Client handler is in charge of abstracting away the TcpClient work
    /// This class has no logic other than to send and receive messages.
    /// </summary>
    public sealed class ClientHandler : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientHandler));
        private static int totalListenerThreads;
        private readonly User clientUser;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly TcpClient tcpClient;

        public ClientHandler(User user, TcpClient client)
        {
            tcpClient = client;
            clientUser = user;
            Log.Info("New client handler created");
        }

        public User ClientUser
        {
            get { return clientUser; }
        }

        public void Dispose()
        {
            tcpClient.Close();
        }

        public event EventHandler<MessageEventArgs> OnNewMessage
        {
            add { messageReceiver.OnNewMessage += value; }
            remove { messageReceiver.OnNewMessage -= value; }
        }

        public void CreateListenerThreadForClient()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(ClientUser, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };
            messageListenerThread.Start();
        }

        public void SendMessage(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
            Log.Debug("Sent message with identifier " + message.Identifier + " to user with id " + ClientUser.UserId);
        }
    }
}