using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequest : IMessage
    {
        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
        }

        public Contribution Contribution { get; set; }
    }
}