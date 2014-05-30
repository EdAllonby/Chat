using System;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClasses
{
    /// <summary>
    /// This is in charge of abstracting away the TcpClient work for sending and receiving <see cref="IMessage"/>s.
    /// This class has no logic other than to send and receive messages to and from a <see cref="NetworkStream"/>.
    /// This class is identified by the <see cref="clientUserId"/>
    /// </summary>
    public sealed class ConnectionHandler : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConnectionHandler));
        private static int totalListenerThreads;
        private readonly int clientUserId;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly TcpClient tcpClient;

        public ConnectionHandler(int userId, TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            clientUserId = userId;
            Log.Info("New connection handler created");
            CreateListenerThread();
            messageReceiver.MessageReceived += OnMessageReceiverMessageReceived;
        }

        void OnMessageReceiverMessageReceived(object sender, MessageEventArgs e)
        {
            MessageReceived(sender, e);
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Sends an <see cref="IMessage"/> across the <see cref="ConnectionHandler"/>'s <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(IMessage message)
        {
            Contract.Requires(message != null);

            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
            Log.DebugFormat("Sent message with identifier {0} to user with id {1}", message.MessageIdentifier, clientUserId);
        }

        private void CreateListenerThread()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(clientUserId, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };

            messageListenerThread.Start();
        }

        public void Dispose()
        {
            tcpClient.Close();
        }
    }
}