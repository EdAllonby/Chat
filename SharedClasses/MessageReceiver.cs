using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;
using log4net;
using SharedClasses.Message;
using SharedClasses.Serialiser;
using SharedClasses.Serialiser.MessageSerialiser;

namespace SharedClasses
{
    /// <summary>
    /// Listens for incoming messages from the tcp connection. When a new <see cref="IMessage" /> is received,
    /// it will then fire off an <see cref="MessageReceived" /> event where subscribers will be notified.
    /// </summary>
    public sealed class MessageReceiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageReceiver));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        /// <summary>
        /// Fires a <see cref="MessageEventArgs"/> encapsulating an <see cref="IMessage"/> when a new message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientUserId"></param>
        /// <param name="tcpClient"></param>
        public void ReceiveMessages(int clientUserId, TcpClient tcpClient)
        {
            Contract.Requires(clientUserId > 0);
            Contract.Requires(tcpClient != null);

            try
            {
                while (true)
                {
                    MessageIdentifier messageIdentifier =
                        messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

                    ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

                    IMessage message = serialiser.Deserialise(tcpClient.GetStream());

                    OnMessageReceived(new MessageEventArgs(message));
                }
            }
            catch (IOException)
            {
                Log.Info("Detected client disconnection, sending ClientDisconnection object to Server");
                IMessage message = new ClientDisconnection(clientUserId);
                MessageReceived(this, new MessageEventArgs(message));
            }
        }

        private void OnMessageReceived(MessageEventArgs messageEventArgs)
        {
            EventHandler<MessageEventArgs> messageReceivedCopy = MessageReceived;

            if (messageReceivedCopy != null)
            {
                messageReceivedCopy(this, messageEventArgs);
            }
        }
    }
}