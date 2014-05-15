using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClasses
{
    /// <summary>
    /// Class to get the correct serialiser for the given <see cref="IMessage" /> identifier
    /// </summary>
    public sealed class SerialiserFactory
    {
        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage" /> object identifier
        /// defined in the <see cref="MessageIdentifierSerialiser" /> class
        /// </summary>
        /// <typeparam name="T">="T">The type of message that will be sent</typeparam>
        /// <returns>The serialiser used to serialise and deserialise the message</returns>
        public ISerialiser<T> GetSerialiser<T>() where T : IMessage
        {
            return SerialiserRegistry.SerialisersByMessageType[typeof (T)] as ISerialiser<T>;
        }

        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage" /> object identifier
        /// defined in the <see cref="MessageIdentifierSerialiser" /> class
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public ISerialiser GetSerialiser(MessageNumber identifier)
        {
            return SerialiserRegistry.SerialisersByMessageIdentifier[identifier];
        }
    }
}