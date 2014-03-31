using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    public class ContributionRequestSerialiser : ISerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        public void Serialise(IMessage contributionRequestMessage, NetworkStream stream)
        {
            MessageUtilities.SerialiseMessageIdentifier(contributionRequestMessage.GetMessageIdentifier(), stream);
            var message = contributionRequestMessage as ContributionRequest;
            
            if (message != null)
            {
                Log.Debug("Waiting for contribution request message to serialise");
                serialiser.Serialise(message.Contribution, stream);
                Log.Info("Contribution request message serialised");
            }
            else
            {
                Log.Warn("No message to be serialised, message object is null");
            }
        }

        public IMessage Deserialise(NetworkStream stream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            var request = new ContributionRequest(serialiser.Deserialise(stream));
            Log.Info("Contribution request message deserialised");
            return request;
        }
    }
}