using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification : IMessage
    {
        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
        }

        public Contribution Contribution { get; set; }

        public string GetMessage()
        {
            return Contribution.GetMessage();
        }

        public int GetMessageIdentifier()
        {
            return MessageUtilities.GetMessageIdentifier(typeof (ContributionNotification));
        }
    }
}