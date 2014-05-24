using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    public sealed class ParticipationRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserRepository));

        private readonly List<Participation> participations = new List<Participation>();

        /// <summary>
        /// Adds a <see cref="Participation"/> entity to the repository
        /// </summary>
        /// <param name="participation">The <see cref="Participation"/> entity to add to the repository</param>
        public void AddParticipation(Participation participation)
        {
            participations.Add(participation);
        }

        public void AddParticipations(IEnumerable<Participation> newParticipations)
        {
            Contract.Requires(newParticipations != null);

            foreach (Participation participation in newParticipations)
            {
                participations.Add(participation);
                Log.DebugFormat("Participation with User Id {0} and Conversation Id {1} added to user repository", participation.UserId, participation.ConversationId);
            }
        }

        /// <summary>
        /// Checks whether a conversation exists with a group of participants.
        /// </summary>
        /// <param name="participantIds">The group of participants to check if a <see cref="Conversation"/> exists for.</param>
        /// <returns>Whether or not a <see cref="Conversation"/> exists with the group of participants.</returns>
        [Pure]
        public bool DoesConversationWithUsersExist(IEnumerable<int> participantIds)
        {
            Dictionary<int, List<int>> userIdsIndexedByConversationId = GetUserIdsIndexedByConversationId();

            return userIdsIndexedByConversationId
                .Select(conversationKeyValuePair => conversationKeyValuePair
                    .Value.HasSameElementsAs(participantIds))
                .Any(isConversation => isConversation);
        }

        public IEnumerable<Participation> GetParticipationsByConversationId(int conversationId)
        {
            return participations.Where(participation => participation.ConversationId == conversationId).ToList();
        }

        /// <summary>
        /// Returns the <see cref="Conversation"/> Id that exists for the group of participants.
        /// </summary>
        /// <param name="participantIds">The Ids of the participants.</param>
        /// <returns>The <see cref="Conversation"/> Id that the participants are in.</returns>
        public int GetConversationIdByParticipantsId(IEnumerable<int> participantIds)
        {
            Dictionary<int, List<int>> userIdsIndexedByConversationId = GetUserIdsIndexedByConversationId();

            return userIdsIndexedByConversationId
                .Where(userIds => userIds.Value.HasSameElementsAs(participantIds))
                .Select(userIds => userIds.Key)
                .FirstOrDefault();
        }

        public IEnumerable<int> GetAllConversationIdsByUserId(int userId)
        {
            return
                from participation
                    in participations
                where participation.UserId == userId
                select participation.ConversationId;
        }

        /// <summary>
        /// Returns all of the <see cref="Participation"/> entities held in the repository.
        /// </summary>
        /// <returns>The collection of <see cref="Participation"/> held in the repository.</returns>
        public IEnumerable<Participation> GetAllParticipations()
        {
            return participations;
        }

        private Dictionary<int, List<int>> GetUserIdsIndexedByConversationId()
        {
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (Participation participation in participations)
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