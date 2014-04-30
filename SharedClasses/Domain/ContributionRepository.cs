using System.Collections.Generic;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    public sealed class ContributionRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContributionRepository));

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
            Log.Debug("Contribution with Id " + contribution.ContributionId + " added to user repository");
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