using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    internal class LoginResponseSerialiser : ISerialiser<LoginResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginResponseSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(LoginResponse message, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                Log.Info("Attempt to serialise LoginResponse and send to stream");
                binaryFormatter.Serialize(stream, message);
                Log.Info("LoginResponse serialised and sent to network stream");
            }
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((LoginResponse) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = (LoginResponse) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a LoginResponse object");
            return loginResponse;
        }
    }
}