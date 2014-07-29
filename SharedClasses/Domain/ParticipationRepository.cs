using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SharedClasses.Domain
{
    public sealed class ParticipationRepository : Repository<Participation>
    {
        public event EventHandler<IEnumerable<Participation>> ParticipationsAdded;

        /// <summary>
        /// Adds a group of participations 
        /// </summary>
        /// <param name="participationsToAdd"></param>
        public void AddParticipations(IEnumerable<Participation> participationsToAdd)
        {
            Contract.Requires(participationsToAdd != null);


            IEnumerable<Participation> participations = participationsToAdd as Participation[] ??
                                                        participationsToAdd.ToArray();

            foreach (Participation participation in participations)
            {
                AddParticipationToRepository(participation);
            }

            OnParticipationsAdded(participations);
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
                EntitiesIndexedById.Values.Where(participation => participation.ConversationId == conversationId).ToList();
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
            return from participation in EntitiesIndexedById.Values
                where participation.UserId == userId
                select participation.ConversationId;
        }

        private void AddParticipationToRepository(Participation participation)
        {
            Contract.Requires(participation != null);
            Contract.Requires(participation.Id > 0);

            EntitiesIndexedById.TryAdd(participation.Id, participation);

            Log.DebugFormat("Participation with User Id {0} and Conversation Id {1} added to user repository",
                participation.UserId, participation.ConversationId);
        }

        private Dictionary<int, List<int>> GetUserIdsIndexedByConversationId()
        {
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (Participation participation in EntitiesIndexedById.Values)
            {
                if (!userIdsIndexedByConversationId.ContainsKey(participation.ConversationId))
                {
                    userIdsIndexedByConversationId[participation.ConversationId] = new List<int>();
                }

                userIdsIndexedByConversationId[participation.ConversationId].Add(participation.UserId);
            }

            return userIdsIndexedByConversationId;
        }

        private void OnParticipationsAdded(IEnumerable<Participation> participation)
        {
            EventHandler<IEnumerable<Participation>> participationsAddedCopy = ParticipationsAdded;

            if (participationsAddedCopy != null)
            {
                ParticipationsAdded(this, participation);
            }
        }
    }
}