using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    public delegate void ParticipationChangedHandler(Participation participation);

    public delegate void ParticipationsChangedHandler(IEnumerable<Participation> participations);

    public sealed class ParticipationRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserRepository));

        private readonly Dictionary<int, Participation> participationsIndexedById = new Dictionary<int, Participation>();

        public event ParticipationChangedHandler ParticipationAdded = delegate { };
        public event ParticipationsChangedHandler ParticipationsAdded = delegate { };

        /// <summary>
        /// Adds a <see cref="Participation"/> entity to the repository
        /// </summary>
        /// <param name="participation">The <see cref="Participation"/> entity to add to the repository. Participation must not be null and must have an id greater than 0.</param>
        public void AddParticipation(Participation participation)
        {
            Contract.Requires(participation != null);

            AddParticipationToRepository(participation);

            ParticipationAdded(participation);
        }

        /// <summary>
        /// Adds a group of participations 
        /// </summary>
        /// <param name="participationsToAdd"></param>
        public void AddParticipations(IEnumerable<Participation> participationsToAdd)
        {
            Contract.Requires(participationsToAdd != null);


            IEnumerable<Participation> participationsEnumerable = participationsToAdd as Participation[] ??
                                                                  participationsToAdd.ToArray();

            foreach (Participation participation in participationsEnumerable)
            {
                AddParticipationToRepository(participation);
            }

            ParticipationsAdded(participationsEnumerable);
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

            return userIdsIndexedByConversationId.Select(conversationKeyValuePair => conversationKeyValuePair
                .Value.HasSameElementsAs(participantIds))
                .Any(isConversation => isConversation);
        }

        public IEnumerable<Participation> GetParticipationsByConversationId(int conversationId)
        {
            return
                participationsIndexedById.Values.Where(participation => participation.ConversationId == conversationId).ToList();
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
            return from participation in participationsIndexedById.Values
                where participation.UserId == userId
                select participation.ConversationId;
        }

        /// <summary>
        /// Returns all of the <see cref="Participation"/> entities held in the repository.
        /// </summary>
        /// <returns>The collection of <see cref="Participation"/> held in the repository.</returns>
        public IEnumerable<Participation> GetAllParticipations()
        {
            return participationsIndexedById.Values;
        }

        private void AddParticipationToRepository(Participation participation)
        {
            Contract.Requires(participation != null);
            Contract.Requires(participation.ParticipationId > 0);

            participationsIndexedById.Add(participation.ParticipationId, participation);

            Log.DebugFormat("Participation with User Id {0} and Conversation Id {1} added to user repository",
                participation.UserId, participation.ConversationId);
        }

        private Dictionary<int, List<int>> GetUserIdsIndexedByConversationId()
        {
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (Participation participation in participationsIndexedById.Values)
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