using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginResponse" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    public sealed class LoginResponseSerialiser : ISerialiser<LoginResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginResponseSerialiser));

        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(LoginResponse message, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                Log.Info("Attempt to serialise LoginResponse and send to stream");
                userSerialiser.Serialise(message.User, stream);
                Log.Info("LoginResponse serialised and sent to network stream");
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginResponse)loginRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var loginResponse = new LoginResponse(userSerialiser.Deserialise(networkStream));
            Log.Info("Network stream has received data and deserialised to a LoginResponse object");
            return loginResponse;
        }
    }
}