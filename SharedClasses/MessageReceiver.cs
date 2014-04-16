using System;
using System.IO;
using log4net;
using SharedClasses.Protocol;

namespace SharedClasses
{
    /// <summary>
    ///     A class to listen for incoming messages from the wire. When a new <see cref="IMessage" /> is received,
    ///     it will then fire off an <see cref="OnNewMessage" /> event where subscribers will be notified.
    /// </summary>
    public class MessageReceiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MessageReceiver));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public event EventHandler<MessageEventArgs> OnNewMessage;

        public void ReceiveMessages(ConnectedClient connectedClient)
        {
            try
            {
                while (true)
                {
                    int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(connectedClient.TcpClient.GetStream());

                    ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

                    IMessage message = serialiser.Deserialise(connectedClient.TcpClient.GetStream());

                    OnNewMessage(this, new MessageEventArgs(message, connectedClient));
                }
            }
            catch (IOException ioException)
            {
                Log.Error("Couldn't send message across stream, removing client from server", ioException);
                IMessage message = new ClientDisconnection();
                OnNewMessage(this, new MessageEventArgs(message, connectedClient));
            }
        }
    }
}