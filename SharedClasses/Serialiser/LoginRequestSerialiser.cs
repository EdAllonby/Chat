using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginRequest" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    public sealed class LoginRequestSerialiser : ISerialiser<LoginRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginRequestSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        public void Serialise(LoginRequest message, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                userSerialiser.Serialise(message.User, stream);
                Log.InfoFormat("{0} message serialised and sent to network stream", message.Identifier);
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginRequest) loginRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            User user = userSerialiser.Deserialise(networkStream);
            var loginRequest = new LoginRequest(user.Username);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginRequest.Identifier);
            return loginRequest;
        }
    }
}