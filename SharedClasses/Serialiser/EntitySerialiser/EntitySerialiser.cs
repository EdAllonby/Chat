using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Serialises an <see cref="IEntity" />across the stream.
    /// </summary>
    /// <typeparam name="T">The <see cref="IEntity" /> to serialise.</typeparam>
    internal sealed class EntitySerialiser<T> where T : IEntity
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EntitySerialiser<T>));
        private readonly ISerialiser serialiser = new BinarySerialiser();

        /// <summary>
        /// Serialises an <see cref="IEntity" /> through the <see cref="NetworkStream" />.
        /// </summary>
        /// <param name="networkStream">The <see cref="NetworkStream" /> to serialise the <see cref="IEntity" /> across.</param>
        /// <param name="entity">The <see cref="IEntity" /> to serialise.</param>
        public void Serialise(NetworkStream networkStream, T entity)
        {
            serialiser.Serialise(networkStream, entity);
            Log.DebugFormat("{0} entity serialised and sent to network stream", entity);
        }

        /// <summary>
        /// Deserialises an <see cref="IEntity" /> from the <see cref="NetworkStream" />.
        /// </summary>
        /// <param name="networkStream">The <see cref="NetworkStream" /> containing the serialised <see cref="IEntity" />.</param>
        /// <returns>The deserialised <see cref="IEntity" />.</returns>
        public T Deserialise(NetworkStream networkStream)
        {
            var entity = (T) serialiser.Deserialise(networkStream);
            Log.DebugFormat("Network stream has received data and deserialised to a {0} entity", entity);
            return entity;
        }
    }
}