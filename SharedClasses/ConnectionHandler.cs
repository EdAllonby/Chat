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
    /// This is in charge of abstracting away the TcpClient work
    /// This class has no logic other than to send and receive messages.
    /// </summary>
    public sealed class ConnectionHandler : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConnectionHandler));
        private static int totalListenerThreads;
        private readonly int clientUserId;

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly TcpClient tcpClient;

        public ConnectionHandler(int userId, TcpClient client)
        {
            tcpClient = client;
            clientUserId = userId;
            Log.Info("New client handler created");
            CreateListenerThreadForClient();
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

        private void CreateListenerThreadForClient()
        {
            var messageListenerThread = new Thread(() => messageReceiver.ReceiveMessages(clientUserId, tcpClient))
            {
                Name = "ReceiveMessageThread" + (totalListenerThreads++)
            };
            messageListenerThread.Start();
        }

        public void SendMessage(IMessage message)
        {
            Contract.Requires(message != null);

            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
            Log.Debug("Sent message with identifier " + message.Identifier + " to user with id " + clientUserId);
        }
    }
}