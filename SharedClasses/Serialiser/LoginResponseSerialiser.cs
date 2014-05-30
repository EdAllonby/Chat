using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginResponse" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class LoginResponseSerialiser : Serialiser<LoginResponse>
    {
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        protected override void Serialise(LoginResponse message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.MessageIdentifier, networkStream);
            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = (LoginResponse)binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginResponse.MessageIdentifier);
            return loginResponse;
        }
    }
}