using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginRequest" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class LoginRequestSerialiser : Serialiser<LoginRequest>
    {
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        protected override void Serialise(LoginRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            userSerialiser.Serialise(message.User, stream);
            Log.InfoFormat("{0} message serialised and sent to network stream", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            User user = userSerialiser.Deserialise(stream);
            var loginRequest = new LoginRequest(user.Username);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginRequest.Identifier);
            return loginRequest;
        }
    }
}