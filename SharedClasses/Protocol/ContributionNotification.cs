using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification
    {
        public Contribution Contribution { get; set; }

        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
        }
    }
}