using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
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