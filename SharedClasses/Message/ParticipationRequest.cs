using System;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages <see cref="Participation" />s related by conversation Id for the Server to send to the Client
    /// </summary>
    [Serializable]
    public sealed class ParticipationRequest : IMessage
    {
        public ParticipationRequest(Participation participation)
        {
            Participation = participation;
        }

        public Participation Participation { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.ParticipationRequest;
    }
}