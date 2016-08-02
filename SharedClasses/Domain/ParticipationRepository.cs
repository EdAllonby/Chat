using System.Collections.Generic;
using System.Linq;

namespace SharedClasses.Domain
{
    public sealed class ParticipationRepository : EntityRepository<Participation>
    {
        /// <summary>
        /// Checks whether a conversation exists with a group of participants.
        /// </summary>
        /// <param name="userIds">The group of participants to check if a <see cref="Conversation" /> exists for.</param>
        /// <returns>Whether or not a <see cref="Conversation" /> exists with the group of participants.</returns>
        public bool DoesConversationWithUsersExist(IEnumerable<int> userIds)
        {
            Dictionary<int, List<int>> userIdsIndexedByConversationId = GetUserIdsIndexedByConversationId();

            return userIdsIndexedByConversationId.Select(ids => ids.Value.HasSameElementsAs(userIds)).Any(isConversation => isConversation);
        }

        /// <summary>
        /// Gets all <see cref="Participation" /> objects that match the conversation Id.
        /// </summary>
        /// <param name="conversationId">The Id of the conversation all returning <see cref="Participation" /> objects should have.</param>
        /// <returns>The <see cref="Participation" /> objects that match the conversation id.</returns>
        public List<Participation> GetParticipationsByConversationId(int conversationId)
        {
            return GetAllEntities().Where(participation => participation.ConversationId == conversationId).ToList();
        }

        /// <summary>
        /// Returns the <see cref="Conversation" /> Id that exists for the group of participants.
        /// </summary>
        /// <param name="userIds">The Ids of the participants.</param>
        /// <returns>The <see cref="Conversation" /> Id that the participants are in.</returns>
        public int GetConversationIdByUserIds(IEnumerable<int> userIds)
        {
            Dictionary<int, List<int>> userIdsIndexedByConversationId = GetUserIdsIndexedByConversationId();

            return userIdsIndexedByConversationId.Where(ids => ids.Value.HasSameElementsAs(userIds)).Select(ids => ids.Key).FirstOrDefault();
        }

        public IEnumerable<int> GetAllConversationIdsByUserId(int userId)
        {
            return from participation in GetAllEntities()
                where participation.UserId.Equals(userId)
                select participation.ConversationId;
        }

        /// <summary>
        /// Finds the specific Participation object that matches the userId and conversationId.
        /// </summary>
        /// <param name="userId">The Id of the user to match.</param>
        /// <param name="conversationId">The Id of the conversation to match.</param>
        /// <returns>The <see cref="Participation" /> that matches the user Id and conversation Id.</returns>
        public Participation GetParticipationByUserIdandConversationId(int userId, int conversationId)
        {
            foreach (Participation possibleParticipation in GetAllEntities())
            {
                if (possibleParticipation.UserId.Equals(userId) && possibleParticipation.ConversationId.Equals(conversationId))
                {
                    return possibleParticipation;
                }
            }

            return null;
        }

        private Dictionary<int, List<int>> GetUserIdsIndexedByConversationId()
        {
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (Participation participation in GetAllEntities())
            {
                if (!userIdsIndexedByConversationId.ContainsKey(participation.ConversationId))
                {
                    userIdsIndexedByConversationId[participation.ConversationId] = new List<int>();
                }

                userIdsIndexedByConversationId[participation.ConversationId].Add(participation.UserId);
            }

            return userIdsIndexedByConversationId;
        }
    }
}