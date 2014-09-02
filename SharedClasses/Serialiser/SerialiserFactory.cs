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
        /// <typeparam name="T">="T">The type of message that will be sent.</typeparam>
        /// <returns>The serialiser used to serialise and deserialise the message.</returns>
        public static Serialiser<T> GetSerialiser<T>() where T : IMessage
        {
            return SerialiserRegistry.SerialisersByMessageType[typeof (T)] as Serialiser<T>;
        }

        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage" /> object identifier
        /// defined in the <see cref="MessageIdentifierSerialiser" /> class.
        /// </summary>
        /// <param name="identifier">The unique name of the <see cref="IMessage"/>.</param>
        /// <returns>The serialiser to get for the <see cref="IMessage"/>.</returns>
        public static ISerialiser GetSerialiser(MessageIdentifier identifier)
        {
            if (identifier == MessageIdentifier.UnrecognisedMessage)
            {
                throw new ArgumentException(String.Format("A serialiser does not exist for message type {0}.", identifier));
            }

            return SerialiserRegistry.SerialisersByMessageIdentifier[identifier];
        }
    }
}