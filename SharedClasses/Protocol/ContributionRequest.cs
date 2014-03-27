using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequest
    {
        public Contribution Contribution { get; set; }
        public int MessageType = 1;

        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
            Contribution.MessageType = MessageType;
        }
    }
}