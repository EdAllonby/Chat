using System;
using System.Net.Sockets;
using SharedClasses.Protocol;

namespace SharedClasses
{
    public class MessageReceiver
    {
        public event EventHandler<MessageEventArgs> OnNewMessage;

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public void ReceiveMessages(NetworkStream stream)
        {
            while (true)
            {
                int messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(stream);

                ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

                IMessage message = serialiser.Deserialise(stream);

                OnNewMessage(this, new MessageEventArgs(message));
            }
        }
    }
}