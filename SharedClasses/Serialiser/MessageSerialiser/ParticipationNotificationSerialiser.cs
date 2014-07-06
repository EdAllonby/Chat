using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    internal sealed class ParticipationNotificationSerialiser : Serialiser<ParticipationNotification>
    {
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var participationNotification = new ParticipationNotification(participationSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", participationNotification.MessageIdentifier);
            return participationNotification;
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