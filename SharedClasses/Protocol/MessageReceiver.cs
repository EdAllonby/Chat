using System;
using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     A class to listen for incoming messages from the wire. When a new <see cref="IMessage" /> is received,
    ///     it will then fire off an <see cref="OnNewMessage" /> event where subscribers will be notified.
    /// </summary>
    public class MessageReceiver
    {
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        public event EventHandler<MessageEventArgs> OnNewMessage;

        public void ReceiveMessages(NetworkStream stream)
        {
            while (true)
            {
                int messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(stream);

                ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

                IMessage message = serialiser.Deserialise(stream);

                OnNewMessage(this, new MessageEventArgs(message));
            }
        }
    }
}