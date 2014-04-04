namespace SharedClasses.Protocol
{
    /// <summary>
    /// <see cref="IMessage"/> interface is used to group the message objects into one type.
    /// This is used to make a general <see cref="ISerialiser"/> object for use with <see cref="SerialiserFactory"/>
    /// An IMessage is guaranteed to get its identifier.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The identifier associated with the particular IMessage.
        /// </summary>
        int Identifier { get; }
    }
}