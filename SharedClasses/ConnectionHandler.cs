using System;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClasses
{
    /// <summary>
    /// This is in charge of abstracting away the TcpClient work for sending and receiving <see cref="IMessage" />s.
    /// This class has no logic other than to send and receive messages to and from a <see cref="NetworkStream" />.
    /// This class is identified by the <see cref="clientUserId" />.
    /// </summary>
    public sealed class ConnectionHandler : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConnectionHandler));
        private static int totalListenerThreads;

        private readonly int clientUserId;
        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly object messageSenderLock = new object();
        private readonly TcpClient tcpClient;

        /// <summary>
        /// Initialises the object so it can begin to send and receive <see cref="IMessage" />s through <see cref="tcpClient" />.
        /// </summary>
        /// <param name="clientUserId">A unique value that identifies the client.</param>
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
        /// Sends an <see cref="IMessage" /> across the <see cref="ConnectionHandler" />'s <see cref="NetworkStream" />.
        /// </summary>
        /// <param name="message">The message to send across the socket connection defined for this object.</param>
        public void SendMessage(IMessage message)
        {
            lock (messageSenderLock)
            {
                IMessageSerialiser messageSerialiser = SerialiserFactory.GetSerialiser(message.MessageIdentifier);
                messageSerialiser.Serialise(tcpClient.GetStream(), message);
                Log.DebugFormat($"Sent message with identifier {message.MessageIdentifier} to user with id {clientUserId}");
            }
        }

        private void CreateListenerThread()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(clientUserId, tcpClient))
            {
                Name = "ReceiveMessageThread" + totalListenerThreads++
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