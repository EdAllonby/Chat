using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginResponse" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class LoginResponseSerialiser : Serialiser<LoginResponse>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(LoginResponse message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.Serialise(networkStream, message.MessageIdentifier);
            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = (LoginResponse) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginResponse.MessageIdentifier);
            return loginResponse;
        }
    }
}