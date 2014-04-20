namespace SharedClasses.Protocol
{
    /// <summary>
    /// Class to get the correct serialiser for the given <see cref="IMessage" /> identifier
    /// </summary>
    public class SerialiserFactory
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
        public ISerialiser GetSerialiser(int identifier)
        {
            return SerialiserRegistry.GetSerialisersByMessageIdentifier(identifier);
        }
    }
}