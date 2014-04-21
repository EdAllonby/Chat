using System.Net.Sockets;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ContributionRequest" /> message
    /// </summary>
    public class ContributionRequestSerialiser : ISerialiser<ContributionRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ContributionRequestSerialiser));

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly ContributionSerialiser serialiser = new ContributionSerialiser();

        #region Serialise

        public void Serialise(ContributionRequest contributionRequest, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(contributionRequest.Identifier, stream);

            Log.Debug("Waiting for contribution request message to serialise");
            serialiser.Serialise(contributionRequest.Contribution, stream);
            Log.Info("Contribution request message serialised");
        }

        public void Serialise(IMessage contributionRequestMessage, NetworkStream stream)
        {
            Serialise((ContributionRequest) contributionRequestMessage, stream);
        }

        #endregion

        #region Deserialise

        public IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for a contribution request message to deserialise");
            var request = new ContributionRequest(serialiser.Deserialise(networkStream));
            Log.Info("Contribution request message deserialised");
            return request;
        }

        #endregion
    }
}