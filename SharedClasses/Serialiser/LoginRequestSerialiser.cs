using System.Net.Sockets;
using log4net;
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

                Log.Info("Attempt to serialise LoginRequest and send to stream");
                userSerialiser.Serialise(message.User, stream);
                Log.Info("LoginRequest serialised and sent to network stream");
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginRequest) loginRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var loginRequest = new LoginRequest(userSerialiser.Deserialise(networkStream));
            Log.Info("Network stream has received data and deserialised to a LoginRequest object");
            return loginRequest;
        }
    }
}