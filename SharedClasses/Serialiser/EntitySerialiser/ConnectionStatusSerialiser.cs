using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    public sealed class ConnectionStatusSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConnectionStatusSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, ConnectionStatus connectionStatus)
        {
            serialiser.Serialise(networkStream, connectionStatus);
        }

        public ConnectionStatus Deserialise(NetworkStream networkStream)
        {
            var connectionStatus = (ConnectionStatus) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to an ConnectionStatus object.");
            return connectionStatus;
        }
    }
}