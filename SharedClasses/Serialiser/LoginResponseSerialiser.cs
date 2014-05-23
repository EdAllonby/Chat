using System.Net.Sockets;
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
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        protected override void Serialise(LoginResponse message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);
            userSerialiser.Serialise(message.User, stream);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var loginResponse = new LoginResponse(userSerialiser.Deserialise(stream));
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginResponse.Identifier);
            return loginResponse;
        }
    }
}