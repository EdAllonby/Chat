using System.Net.Sockets;
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
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, LoginResponse message)
        {
            serialiser.Serialise(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = (LoginResponse) serialiser.Deserialise(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginResponse.MessageIdentifier);
            return loginResponse;
        }
    }
}