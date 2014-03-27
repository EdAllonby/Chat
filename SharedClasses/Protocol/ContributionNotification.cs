using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification
    {
        public Contribution Contribution { get; set; }
        public int MessageType = 2;

        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
            Contribution.MessageType = MessageType;
        }
    }
}