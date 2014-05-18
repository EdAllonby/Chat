using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// The relationship between a User and a Conversation
    /// </summary>
    [Serializable]
    public class Participation
    {
        private readonly int userId;
        private readonly int conversationId;

        public Participation(int userId, int conversationId)
        {
            this.userId = userId;
            this.conversationId = conversationId;
        }

        public int UserId
        {
            get { return userId; }
        }

        public int ConversationId
        {
            get { return conversationId; }
        }
    }
}