using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionSerialiser : ISerialiser<Contribution>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(Contribution clientContribution, NetworkStream stream)
        {
            try
            {
                if (stream.CanWrite)
                {
                    Log.Info("Attempt to serialise Contribution and send to stream");
                    binaryFormatter.Serialize(stream, clientContribution);
                    Log.Info("Contribution serialised and sent to network stream");
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
            }
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((Contribution)message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            try
            {
                if (networkStream.CanRead)
                {
                    var message = (Contribution) binaryFormatter.Deserialize(networkStream);
                    Log.Info("Network stream has received data and deserialised to a Contribution object");
                    return message;
                }
            }
            catch (IOException ioException)
            {
                Log.Error("connection lost between the client and the server", ioException);
                networkStream.Close();
            }
            return new Contribution(string.Empty);
        }
    }
}