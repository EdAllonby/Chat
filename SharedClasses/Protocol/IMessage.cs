namespace SharedClasses.Protocol
{
    /// <summary>
    /// <see cref="IMessage"/> interface is used to group the message objects into one type.
    /// This is used to make a general <see cref="ISerialiser"/> object for use with <see cref="SerialiserFactory"/>
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The contents of the <see cref="IMessage"/> message
        /// </summary>
        /// <returns>A string of the message</returns>
        string GetMessage();

        int GetMessageIdentifier();
    }
}