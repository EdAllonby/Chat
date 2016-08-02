using System.Net.Sockets;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Serialise and Deserialise an <see langword="object" /> across a <see cref="NetworkStream" />.
    /// </summary>
    public interface ISerialiser
    {
        /// <summary>
        /// Serialise an <see langword="object" /> across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to serialise across.</param>
        /// <param name="serialisableObject">The <see langword="object" /> to serialise.</param>
        void Serialise(NetworkStream serialisationStream, object serialisableObject);

        /// <summary>
        /// Deserialise an <see langword="object" /> send across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to deserialise.</param>
        /// <returns>An <see langword="object" /> that has been deserialised.</returns>
        object Deserialise(NetworkStream serialisationStream);
    }
}