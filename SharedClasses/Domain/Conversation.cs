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
        private readonly int id;

        public Conversation(int id, User firstParticipant, User secondParticipant)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.id = id;

            FirstParticipant = firstParticipant;
            SecondParticipant = secondParticipant;
        }

        /// <summary>
        /// Each Conversation gets a unique ID
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// The User that initiates the conversion
        /// </summary>
        public User FirstParticipant { get; private set; }

        /// <summary>
        /// The User who <see cref="FirstParticipant"/> wants to talk to
        /// </summary>
        public User SecondParticipant { get; private set; }

        /// <summary>
        /// A list of <see cref="Contributions"/> of the Conversation
        /// </summary>
        public IList<Contribution> Contributions { get; set; }
    }
}