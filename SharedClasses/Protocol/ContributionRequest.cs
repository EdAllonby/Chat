using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     A <see cref="Contribution"/> packaged as a Request for the client to send to the server
    /// </summary>
    public class ContributionRequest : IMessage
    {
        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof (ContributionRequest)];
        }

        public Contribution Contribution { get; set; }
        public int Identifier { get; private set; }
    }
}