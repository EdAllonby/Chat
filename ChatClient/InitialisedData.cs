using SharedClasses.Message;

namespace ChatClient
{
    /// <summary>
    /// Holds the data needed for the client to get initialised correctly.
    /// </summary>
    public sealed class InitialisedData
    {
        public InitialisedData(int userId, UserSnapshot userSnapshot, ConversationSnapshot conversationSnapshot, ParticipationSnapshot participationSnapshot)
        {
            UserId = userId;
            UserSnapshot = userSnapshot;
            ConversationSnapshot = conversationSnapshot;
            ParticipationSnapshot = participationSnapshot;
        }

        public int UserId { get; private set; }

        public UserSnapshot UserSnapshot { get; private set; }

        public ConversationSnapshot ConversationSnapshot { get; private set; }

        public ParticipationSnapshot ParticipationSnapshot { get; private set; }
    }
}