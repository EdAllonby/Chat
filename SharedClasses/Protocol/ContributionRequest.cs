using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequest
    {
        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
        }

        public Contribution Contribution { get; set; }
    }
}