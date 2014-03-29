using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification
    {
        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
        }

        public Contribution Contribution { get; set; }
    }
}