using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    internal sealed class ParticipationNotificationSerialiser : Serialiser<ParticipationNotification>
    {
        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversationNotification = new ParticipationNotification(participationSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", conversationNotification.MessageIdentifier);
            return conversationNotification;
        }

        protected override void Serialise(ParticipationNotification message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.ParticipationNotification, networkStream);

            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            participationSerialiser.Serialise(networkStream, message.Participation);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }
    }
}