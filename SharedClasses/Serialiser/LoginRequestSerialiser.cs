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

        protected override void Serialise(LoginRequest message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.MessageIdentifier, networkStream);

            userSerialiser.Serialise(message.User, networkStream);
            Log.InfoFormat("{0} message serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            User user = userSerialiser.Deserialise(networkStream);
            var loginRequest = new LoginRequest(user.Username);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginRequest.MessageIdentifier);
            return loginRequest;
        }
    }
}