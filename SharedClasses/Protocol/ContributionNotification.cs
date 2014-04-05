using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public class ContributionNotification : IMessage
    {
        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof (ContributionNotification)];
        }

        public Contribution Contribution { get; private set; }

        public int Identifier { get; private set; }
    }
}