using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class LoginRequestSerialiser : ISerialiser<LoginRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(LoginRequest message, NetworkStream stream)
        {
            try
            {
                if (stream.CanWrite)
                {
                    messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

                    Log.Info("Attempt to serialise LoginRequest and send to stream");
                    binaryFormatter.Serialize(stream, message);
                    Log.Info("LoginRequest serialised and sent to network stream");
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
            }
        }

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            Serialise((LoginRequest) loginRequestMessage, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            try
            {
                if (networkStream.CanRead)
                {
                    var loginRequest = (LoginRequest) binaryFormatter.Deserialize(networkStream);
                    Log.Info("Network stream has received data and deserialised to a LoginRequest object");
                    return loginRequest;
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
                networkStream.Close();
            }
            return new LoginRequest(string.Empty);
        }
    }
}