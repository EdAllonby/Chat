using System.Net.Sockets;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginResponse" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    public sealed class LoginResponseSerialiser : ISerialiser<LoginResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginResponseSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        public void Serialise(LoginResponse message, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                userSerialiser.Serialise(message.User, stream);
                Log.Info("LoginResponse serialised and sent to network stream");
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginResponse) loginRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = new LoginResponse(userSerialiser.Deserialise(networkStream));
            Log.Info("Network stream has received data and deserialised to a LoginResponse object");
            return loginResponse;
        }
    }
}