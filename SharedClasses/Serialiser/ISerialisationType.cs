using System.Net.Sockets;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Serialise and Deserialise an object across a <see cref="NetworkStream" />.
    /// </summary>
    public interface ISerialisationType
    {
        /// <summary>
        /// Serialise an object across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to serialise across.</param>
        /// <param name="serialisableObject">The object to serialise.</param>
        void Serialise(NetworkStream serialisationStream, object serialisableObject);

        /// <summary>
        /// Deserialise an object send across the stream.
        /// </summary>
        /// <param name="serialisationStream">The stream to deserialise.</param>
        /// <returns>An object that has been deserialised.</returns>
        object Deserialise(NetworkStream serialisationStream);
    }
}