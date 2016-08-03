using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Gets the correct <see cref="MessageIdentifier" /> for the snapshot type.
    /// </summary>
    [Serializable]
    public sealed class SnapshotMessageIdentifierFactory
    {
        /// <summary>
        /// A read only version of a <see cref="MessageIdentifier" /> by Entity
        /// <see cref="Type" /> dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, MessageIdentifier> SerialisersByMessageType =
            new Dictionary<Type, MessageIdentifier>
            {
                {typeof (User), MessageIdentifier.UserSnapshot},
                {typeof (Conversation), MessageIdentifier.ConversationSnapshot},
                {typeof (Participation), MessageIdentifier.ParticipationSnapshot},
            };

        /// <summary>
        /// Returns the correct <see cref="MessageIdentifier" /> from the snapshot type.
        /// </summary>
        /// <param name="snapshotType">The snapshot type to get the correct <see cref="MessageIdentifier" />.</param>
        /// <returns>The <see cref="MessageIdentifier" /> linked to the Snapshot Message Type.</returns>
        public MessageIdentifier GetIdentifierBySnapshotType(Type snapshotType)
        {
            return SerialisersByMessageType[snapshotType];
        }
    }
}