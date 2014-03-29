using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification
    {
        public static MessageType MessageType = new MessageType(2);

        public Contribution Contribution { get; set; }

        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
        }
    }
}