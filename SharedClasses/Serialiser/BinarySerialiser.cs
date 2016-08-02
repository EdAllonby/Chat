using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// An implementation of <see cref="ISerialiser" /> to serialise data over a <see cref="NetworkStream" /> in binary
    /// format.
    /// </summary>
    internal sealed class BinarySerialiser : ISerialiser
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// Serialise an object across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to serialise across.</param>
        /// <param name="serialisableObject">The object to serialise.</param>
        public void Serialise(NetworkStream serialisationStream, object serialisableObject)
        {
            binaryFormatter.Serialize(serialisationStream, serialisableObject);
        }

        /// <summary>
        /// Deserialise an object send across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to deserialise.</param>
        /// <returns>An object that has been deserialised.</returns>
        public object Deserialise(NetworkStream serialisationStream)
        {
            return binaryFormatter.Deserialize(serialisationStream);
        }
    }
}