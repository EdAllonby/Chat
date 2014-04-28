using System.Collections.Generic;

namespace SharedClasses.Domain
{
    public class ContributionRepository
    {
        private readonly Dictionary<int, List<Contribution>> contributionsIndexedByConversationId = new Dictionary<int, List<Contribution>>();
        private readonly Dictionary<int, Contribution> contributionsIndexedById = new Dictionary<int, Contribution>();

        public void AddContribution(Contribution contribution)
        {
            if (contributionsIndexedByConversationId.ContainsKey(contribution.ConversationId))
            {
                contributionsIndexedByConversationId[contribution.ConversationId].Add(contribution);
            }
            else
            {
                contributionsIndexedByConversationId.Add(contribution.ConversationId, new List<Contribution> {contribution});
            }

            contributionsIndexedById[contribution.ContributionId] = contribution;
        }

        public void RemoveContributionById(int contributionId)
        {
            // Remove contribution to indices
        }

        public Contribution FindContributionById(int contributionId)
        {
            return contributionsIndexedById[contributionId];
        }

        public IEnumerable<Contribution> FindContributionsByConversation(Conversation conversation)
        {
            return contributionsIndexedByConversationId[conversation.ConversationId];
        }
    }
}