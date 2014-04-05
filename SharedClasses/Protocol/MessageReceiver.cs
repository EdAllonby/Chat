using System;
using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
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