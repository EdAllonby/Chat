using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    internal class UserNotificationSerialiser : ISerialiser<UserNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserNotificationSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(UserNotification message, NetworkStream stream)
        {
            try
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                Log.Info("Attempt to serialise UserNotification and send to stream");
                binaryFormatter.Serialize(stream, message);
                Log.Info("UserNotification serialised and sent to network stream");
            }
            catch (IOException ioException)
            {
                Log.Error("Connection lost between the client and the server", ioException);
            }
        }

        public void Serialise(IMessage userNotification, NetworkStream stream)
        {
            Serialise((UserNotification) userNotification, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            try
            {
                if (networkStream.CanRead)
                {
                    var userNotification = (UserNotification) binaryFormatter.Deserialize(networkStream);
                    Log.Info("Network stream has received data and deserialised to a UserNotification object");
                    return userNotification;
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
                networkStream.Close();
            }
            return null;
        }
    }
}