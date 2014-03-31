using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    public class LoginRequestSerialiser : ISerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(IMessage loginRequestMessage, NetworkStream stream)
        {
            try
            {
                if (stream.CanWrite)
                {
                    MessageUtilities.SerialiseMessageIdentifier(loginRequestMessage.GetMessageIdentifier(), stream);

                    Log.Info("Attempt to serialise LoginRequest and send to stream");
                    binaryFormatter.Serialize(stream, loginRequestMessage);
                    Log.Info("LoginRequest serialised and sent to network stream");
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
            }
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