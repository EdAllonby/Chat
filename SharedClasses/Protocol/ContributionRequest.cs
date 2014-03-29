using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequest
    {
        public static MessageType MessageType = new MessageType(1);

        public Contribution Contribution { get; set; }

        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
        }
    }
}