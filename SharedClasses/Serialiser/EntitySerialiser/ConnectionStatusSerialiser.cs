using System.Diagnostics.Contracts;
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
            Contract.Requires(networkStream != null);
            Contract.Requires(connectionStatus != null);

            serialiser.Serialise(networkStream, connectionStatus);
        }

        public ConnectionStatus Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var connectionStatus = (ConnectionStatus)serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to an ConnectionStatus object.");
            return connectionStatus;
        }
    }
}
