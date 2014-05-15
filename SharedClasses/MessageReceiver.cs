using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;
using log4net;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClasses
{
    /// <summary>
    /// A class to listen for incoming messages from the wire. When a new <see cref="IMessage" /> is received,
    /// it will then fire off an <see cref="OnNewMessage" /> event where subscribers will be notified.
    /// </summary>
    public sealed class MessageReceiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageReceiver));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public event EventHandler<MessageEventArgs> OnNewMessage;

        public void ReceiveMessages(int clientUserId, TcpClient tcpClient)
        {
            Contract.Requires(tcpClient != null);

            try
            {
                while (true)
                {
                    MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

                    ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

                    IMessage message = serialiser.Deserialise(tcpClient.GetStream());

                    OnNewMessage(this, new MessageEventArgs(message, clientUserId));
                }
            }
            catch (IOException)
            {
                Log.Info("Detected client disconnection, sending ClientDisconnection object to Server");
                IMessage message = new ClientDisconnection();
                OnNewMessage(this, new MessageEventArgs(message, clientUserId));
            }
        }
    }
}