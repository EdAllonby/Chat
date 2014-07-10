using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    internal sealed class ParticipationRequestSerialiser : Serialiser<ParticipationRequest>
    {
        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();

        protected override void Serialise(NetworkStream networkStream, ParticipationRequest message)
        {
            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            participationSerialiser.Serialise(networkStream, message.Participation);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var participationRequest = new ParticipationRequest(participationSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", participationRequest.MessageIdentifier);
            return participationRequest;
        }
    }
}