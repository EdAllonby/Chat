using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Gets the correct <see cref="IMessage" /> for the <see cref="EntitySnapshotRequest{TEntity}" />
    /// <see cref="MessageIdentifier" />.
    /// </summary>
    [Serializable]
    public sealed class SnapshotRequestMessageIdentifierFactory
    {
        /// <summary>
        /// A read only version of a <see cref="MessageIdentifier" /> by Entity
        /// <see cref="Type" /> dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, MessageIdentifier> SerialisersByMessageType =
            new Dictionary<Type, MessageIdentifier>
            {
                {typeof (User), MessageIdentifier.UserSnapshotRequest},
                {typeof (Conversation), MessageIdentifier.ConversationSnapshotRequest},
                {typeof (Participation), MessageIdentifier.ParticipationSnapshotRequest},
            };

        /// <summary>
        /// Returns the correct <see cref="MessageIdentifier" /> from the <see cref="EntitySnapshotRequest{T}" /> type.
        /// </summary>
        /// <param name="snapshotType">
        /// The <see cref="EntitySnapshotRequest{T}" /> type to get the correct
        /// <see cref="MessageIdentifier" />.
        /// </param>
        /// <returns>The <see cref="MessageIdentifier" /> linked to the <see cref="EntitySnapshotRequest{T}" /> Type.</returns>
        public MessageIdentifier GetIdentifierBySnapshotType(Type snapshotType)
        {
            return SerialisersByMessageType[snapshotType];
        }
    }
}