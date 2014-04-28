using System;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace Server
{
    /// <summary>
    /// The Client handler is in charge of abstracting away the TcpClient Network work from the <see cref="Server"/>
    /// This class has no logic other than to send and receive messages. The logic is handled by <see cref="Server"/>
    /// </summary>
    public sealed class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientHandler));

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly TcpClient tcpClient;

        private int totalListenerThreads;

        public ClientHandler(TcpClient client)
        {
            tcpClient = client;
            Log.Info("New client handler created");
        }

        public User ClientUser { get; private set; }


        public event EventHandler<MessageEventArgs> OnNewMessage
        {
            add { messageReceiver.OnNewMessage += value; }
            remove { messageReceiver.OnNewMessage -= value; }
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(ClientUser, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };
            messageListenerThread.Start();
        }

        public LoginRequest GetClientLoginCredentials()
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();
            int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());
            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);
            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());
            return loginRequest;
        }

        public void AddUserToClientHandler(User user)
        {
            ClientUser = user;
        }

        public void SendMessage(IMessage message)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            TcpClient client = tcpClient;
            messageSerialiser.Serialise(message, client.GetStream());
            Log.Debug("Sent message with identifier " + message.Identifier + " to user with id " + ClientUser.UserId);
        }
    }
}