using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// Holds the snapshots needed for the client to get initialised correctly.
    /// </summary>
    public sealed class Snapshots
    {
        public Snapshots(UserSnapshot userSnapshot, ConversationSnapshot conversationSnapshot, ParticipationSnapshot participationSnapshot)
        {
            UserSnapshot = userSnapshot;
            ConversationSnapshot = conversationSnapshot;
            ParticipationSnapshot = participationSnapshot;
        }

        public UserSnapshot UserSnapshot { get; private set; }

        public ConversationSnapshot ConversationSnapshot { get; private set; }

        public ParticipationSnapshot ParticipationSnapshot { get; private set; }
    }
}