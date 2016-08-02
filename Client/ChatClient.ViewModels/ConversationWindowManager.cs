using System;
using System.Collections.Generic;
using System.Windows;
using log4net;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// Holds the active conversation windows for the user. Can request to create a new conversation window, or change the
    /// status of a window.
    /// </summary>
    public static class ConversationWindowManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConversationWindowManager));

        private static readonly Dictionary<int, WindowStatus> ConversationWindowStatusesIndexedByConversationId = new Dictionary<int, WindowStatus>();

        public static EventHandler<Conversation> OpenChatWindowRequest;

        /// <summary>
        /// Creates a new chat window if the current chat window is closed.
        /// </summary>
        /// <param name="serviceRegistry">Holds the services for the client.</param>
        /// <param name="conversation">The conversation id of the chat window.</param>
        internal static void CreateConversationWindow(IServiceRegistry serviceRegistry, Conversation conversation)
        {
            // Check if conversation window already exists
            if (GetWindowStatus(conversation.Id) == WindowStatus.Closed)
            {
                Application.Current.Dispatcher.Invoke(() => OnOpenChatWindowRequest(conversation));

                SetWindowStatus(conversation.Id, WindowStatus.Open);
                Log.DebugFormat($"Window with conversation Id {conversation.Id} has been created.");
            }
        }

        /// <summary>
        /// Sets the current status of the conversation window, if no conversation is found, create one.
        /// </summary>
        /// <param name="conversationId">The conversation's ID relating to the window.</param>
        /// <param name="windowStatus">The status of the conversation window we want to set.</param>
        internal static void SetWindowStatus(int conversationId, WindowStatus windowStatus)
        {
            if (!ConversationWindowStatusesIndexedByConversationId.ContainsKey(conversationId))
            {
                ConversationWindowStatusesIndexedByConversationId.Add(conversationId, windowStatus);
            }
            else
            {
                ConversationWindowStatusesIndexedByConversationId[conversationId] = windowStatus;
            }
        }

        /// <summary>
        /// Gets the current window status of the conversation window. If no status is found, return closed.
        /// </summary>
        /// <param name="conversationId">The conversation's ID being queried</param>
        /// <returns>The window status of the selected conversation window</returns>
        private static WindowStatus GetWindowStatus(int conversationId)
        {
            WindowStatus windowStatus;

            bool isFound = ConversationWindowStatusesIndexedByConversationId.TryGetValue(conversationId, out windowStatus);

            return isFound ? windowStatus : WindowStatus.Closed;
        }

        private static void OnOpenChatWindowRequest(Conversation conversation)
        {
            EventHandler<Conversation> openChatWindowRequestCopy = OpenChatWindowRequest;
            if (openChatWindowRequestCopy != null)
            {
                openChatWindowRequestCopy(null, conversation);
            }
        }
    }
}