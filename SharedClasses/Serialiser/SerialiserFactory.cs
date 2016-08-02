using System;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Get the correct serialiser for the given <see cref="IMessage" /> identifier.
    /// </summary>
    public static class SerialiserFactory
    {
        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage" /> object identifier
        /// defined in the <see cref="MessageIdentifierSerialiser" /> class.
        /// </summary>
        /// <param name="identifier">The unique name of the <see cref="IMessage" />.</param>
        /// <returns>The serialiser to get for the <see cref="IMessage" />.</returns>
        public static IMessageSerialiser GetSerialiser(MessageIdentifier identifier)
        {
            if (identifier == MessageIdentifier.UnrecognisedMessage)
            {
                throw new ArgumentException($"A serialiser does not exist for message type {identifier}.");
            }

            return SerialiserRegistry.SerialisersByMessageIdentifier[identifier];
        }
    }
}