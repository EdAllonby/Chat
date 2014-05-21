/**
* Notice: This software is proprietary to CME, its affiliates, partners and/or licensors.
* Unauthorized copying, distribution or use is strictly prohibited.  All rights reserved.
*/

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharedClasses;
using SharedClasses.Domain;

namespace SharedClassesTests
{
    [TestFixture]
    public class RepositoryFactoryTests
    {
        [Test]
        public void AddUserEntityTest()
        {
            var user = new User("User", 2, ConnectionStatus.Connected);
            var userRepository = new UserRepository();
            userRepository.AddEntity(user);
            Assert.AreEqual(user, userRepository.FindEntityByID(user.UserId));
        }
              
        [Test]
        public void AddConversationEntityTest()
        {
            var conversation = new Conversation(1);
            var conversationRepository = new ConversationRepository();
            conversationRepository.AddEntity(conversation);
            Assert.AreEqual(conversation, conversationRepository.FindEntityByID(conversation.ConversationId));
        }

        [Test]
        public void AddUserEntitiesTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 2, ConnectionStatus.Connected);
            var users = new List<User> { user1, user2 };

            var userRepository = new UserRepository();

            userRepository.AddUsers(users);
            Assert.AreEqual(users, userRepository.GetAllEntities().ToList());
        }

        [Test]
        public void AddConversationEntitiesTest()
        {
            var conversation1 = new Conversation(1);
            var conversation2 = new Conversation(2);
            var conversations = new List<Conversation> { conversation1, conversation2 };

            var conversationRepository = new ConversationRepository();

            conversationRepository.AddConversations(conversations);
            Assert.AreEqual(conversations, conversationRepository.GetAllEntities().ToList());
        }

        [Test]
        public void CanNotAddSameUserEntityTwice()
        {
            var user = new User("User", 1, ConnectionStatus.Connected);

            var userRepository = new UserRepository();
            
            userRepository.AddEntity(user);
            userRepository.AddEntity(user);

            var users = new List<User> {user};

            Assert.AreEqual(users, userRepository.GetAllEntities());
        }

        [Test] 
        public void CanNotAddSameConversationEntityTwice()
        {
            var conversation = new Conversation(1);

            var conversationRepository = new ConversationRepository();

            conversationRepository.AddEntity(conversation);
            conversationRepository.AddEntity(conversation);

            var conversations = new List<Conversation> { conversation };

            Assert.AreEqual(conversations, conversationRepository.GetAllEntities());
        }
    }
}