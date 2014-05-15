using System.Collections.Generic;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// Holds the conversation window status for each active conversation in the system.
    /// </summary>
    public class ConversationWindowsStatus
    {
        private readonly Dictionary<int, WindowStatus> conversationWindowStatusesIndexedByConversationID = new Dictionary<int, WindowStatus>();

        /// <summary>
        /// Gets the current window status of the conversation window. If no status is found, return closed.
        /// </summary>
        /// <param name="conversationID">The conversation's ID being queried</param>
        /// <returns>The window status of the selected conversation window</returns>
        public WindowStatus GetWindowStatus(int conversationID)
        {
            WindowStatus windowStatus;

            bool isFound = conversationWindowStatusesIndexedByConversationID.TryGetValue(conversationID, out windowStatus);

            return isFound ? windowStatus : WindowStatus.Closed;
        }

        /// <summary>
        /// Sets the current status of the conversation window, if no conversation is found, create one.
        /// </summary>
        /// <param name="conversationID">The conversation's ID relating to the window</param>
        /// <param name="windowStatus">The status of the conversation window we want to set</param>
        public void SetWindowStatus(int conversationID, WindowStatus windowStatus)
        {
            if (!conversationWindowStatusesIndexedByConversationID.ContainsKey(conversationID))
            {
                conversationWindowStatusesIndexedByConversationID.Add(conversationID, windowStatus);
            }
            else
            {
                conversationWindowStatusesIndexedByConversationID[conversationID] = windowStatus;
            }
        }
    }
}