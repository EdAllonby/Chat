using System.Collections.Generic;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ConversationRequestHandlerTest : MessageHandlerTestFixture
    {
        private readonly ConversationRequestHandler conversationRequestHandler = new ConversationRequestHandler();
        private const int UserIdToConverseWith = 2;

        private ConversationRepository ConversationRepository
        {
            get { return ServiceRegistry.GetService<RepositoryManager>().ConversationRepository; }
        }

        private readonly ConversationRequest conversationRequest = new ConversationRequest(new List<int> {ConnectedUserId, UserIdToConverseWith});

        [TestFixture]
        public class HandleMessageTest : ConversationRequestHandlerTest
        {
            [Test]
            public void AddsConversationToRepository()
            {
                conversationRequestHandler.HandleMessage(conversationRequest, ServiceRegistry);
                Conversation conversationAdded = ConversationRepository.FindEntityById(1);

                Assert.IsNotNull(conversationAdded);
            }

            [Test]
            public void AssignsNewConversationAnId()
            {
                conversationRequestHandler.HandleMessage(conversationRequest, ServiceRegistry);
                Conversation conversationAdded = ConversationRepository.FindEntityById(1);
                Assert.IsTrue(conversationAdded.Id != 0);
            }
        }
    }
}