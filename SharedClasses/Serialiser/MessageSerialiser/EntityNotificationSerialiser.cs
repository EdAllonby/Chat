using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    internal class EntityNotificationSerialiser<T> : MessageSerialiser<EntityNotification<T>> where T : IEntity
    {
        private readonly EntitySerialiser<T> bandSerialiser = new EntitySerialiser<T>();
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();

        /// <summary>
        /// Serialise a <see cref="EntityNotification{T}" /> down the wire.
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server.</param>
        /// <param name="message">The <see cref="EntityNotification{T}" /> to serialise.</param>
        protected override void Serialise(NetworkStream networkStream, EntityNotification<T> message)
        {
            notificationTypeSerialiser.Serialise(networkStream, message.NotificationType);
            bandSerialiser.Serialise(networkStream, message.Entity);
        }

        /// <summary>
        /// Deserialises a <see cref="EntityNotification{T}" /> from the <see cref="NetworkStream" />.
        /// </summary>
        /// <param name="networkStream">
        /// The <see cref="NetworkStream" /> containing the serialised
        /// <see cref="EntityNotification{T}" />.
        /// </param>
        /// <returns>The deserialised <see cref="EntityNotification{T}" />.</returns>
        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);

            var entityNotification = new EntityNotification<T>(bandSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised", entityNotification.MessageIdentifier);

            return entityNotification;
        }
    }
}