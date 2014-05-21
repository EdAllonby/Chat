using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Sends a list of participations that is associated with the <see cref="User" />
    /// </summary>
    [Serializable]
    public class ParticipationSnapshot : IMessage
    {
        public ParticipationSnapshot(IEnumerable<Participation> participations)
        {
            Participations = participations;
        }

        public IEnumerable<Participation> Participations { get; private set; }

        public MessageNumber Identifier
        {
            get { return MessageNumber.ParticipationSnapshot; }
        }
    }
}