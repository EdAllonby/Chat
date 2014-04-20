using System;
using System.Collections.Generic;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A link between two Users where they can both talk to each other privately.
    /// </summary>
    [Serializable]
    public sealed class Conversation
    {
        /// <summary>
        /// Each Conversation gets a unique ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The User to initiate the conversion
        /// </summary>
        public User FirstParticipant { get; set; }

        /// <summary>
        /// The User who is requested by <see cref="FirstParticipant"/>
        /// </summary>
        public User SecondParticipant { get; set; }

        /// <summary>
        /// A list of <see cref="Contributions"/> of the Conversation
        /// </summary>
        public IList<Contribution> Contributions { get; set; }
    }
}