using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionRequest
    {
        public Contribution Contribution { get; set; }

        public ContributionRequest(Contribution contribution)
        {
            Contribution = contribution;
        }
    }
}