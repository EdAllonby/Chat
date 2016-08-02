using SharedClasses.Serialiser;

namespace SharedClasses.Message
{
    /// <summary>
    /// Used to group the message objects into one type.
    /// This is used with <see cref="ISerialiser" /> to create a <see cref="SerialiserFactory" />
    /// An IMessage is guaranteed to implement its own identifier.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The identifier associated with the particular IMessage. Identifiers are found in
        /// <see cref="SharedClasses.MessageIdentifier" />
        /// </summary>
        MessageIdentifier MessageIdentifier { get; }
    }
}