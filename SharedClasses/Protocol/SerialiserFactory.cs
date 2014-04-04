namespace SharedClasses.Protocol
{
    /// <summary>
    /// Class to get the correct serialiser for the given <see cref="IMessage"/> identifier
    /// </summary>
    public class SerialiserFactory
    {

        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage"/> object identifier
        /// defined in the <see cref="MessageIdentifierSerialiser"/> class
        /// </summary>
        /// <param name="messageIdentifier">The type of message that will be sent</param>
        /// <returns>The serialiser used to serialise and deserialise the message</returns>
        public ISerialiser<T> GetSerialiser<T>() where T : IMessage
        {
            return SerialiserRegistry.serialisersByMessageType[typeof (T)] as ISerialiser<T>;
        }

        public ISerialiser GetSerialiser(int messageNumber)
        {
            return SerialiserRegistry.serialisersByMessageNumber[messageNumber];
        }
    }
}