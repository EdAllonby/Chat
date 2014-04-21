using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginRequest" /> object
    /// </summary>
    public class LoginRequestSerialiser : ISerialiser<LoginRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        #region Serialise

        public void Serialise(LoginRequest message, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                Log.Info("Attempt to serialise LoginRequest and send to stream");
                binaryFormatter.Serialize(stream, message);
                Log.Info("LoginRequest serialised and sent to network stream");
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginRequest) loginRequestMessage, stream);
        }

        #endregion

        #region Deserialise

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var loginRequest = (LoginRequest) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a LoginRequest object");
            return loginRequest;
        }

        #endregion
    }
}