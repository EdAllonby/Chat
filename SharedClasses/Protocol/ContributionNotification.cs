using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     A <see cref="Contribution"/> packaged as a Notification for the server to send to clients
    /// </summary>
    public class ContributionNotification : IMessage
    {
        public ContributionNotification(Contribution contribution)
        {
            Contribution = contribution;
            Identifier = MessageNumber.ContributionNotification;
        }

        public Contribution Contribution { get; private set; }

        public int Identifier { get; private set; }
    }
}