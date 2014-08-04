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
    /// This class is identified by the <see cref="clientUserId"/>.
    /// </summary>
    public sealed class ConnectionHandler : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConnectionHandler));
        private static int totalListenerThreads;

        private readonly int clientUserId;
        private readonly object messageSenderLock = new object();

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly TcpClient tcpClient;

        /// <summary>
        /// Initialises the object so it can begin to send and recieve <see cref="IMessage"/>s through <see cref="tcpClient"/>.
        /// </summary>
        /// <param name="clientUserId">The client </param>
        /// <param name="tcpClient"></param>
        public ConnectionHandler(int clientUserId, TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.clientUserId = clientUserId;

            Log.Info("New connection handler created");

            CreateListenerThread();
            messageReceiver.MessageReceived += OnMessageReceiverMessageReceived;
        }

        public void Dispose()
        {
            tcpClient.Close();
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Sends an <see cref="IMessage"/> across the <see cref="ConnectionHandler"/>'s <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="message">The message to send across the socket connection defined for this object.</param>
        public void SendMessage(IMessage message)
        {
            Contract.Requires(message != null);

            lock (messageSenderLock)
            {
                ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
                messageSerialiser.Serialise(tcpClient.GetStream(), message);
                Log.DebugFormat("Sent message with identifier {0} to user with id {1}", message.MessageIdentifier, clientUserId);
            }
        }

        private void CreateListenerThread()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(clientUserId, tcpClient.GetStream()))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };

            messageListenerThread.Start();
        }

        private void OnMessageReceiverMessageReceived(object sender, MessageEventArgs e)
        {
            EventHandler<MessageEventArgs> messageReceivedCopy = MessageReceived;

            if (messageReceivedCopy != null)
            {
                messageReceivedCopy(sender, e);
            }
        }
    }
}