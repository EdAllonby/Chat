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
            var user = new User("User", 2);
            var repositoryFactory = new RepositoryFactory();
            repositoryFactory.GetRepository<User>().AddEntity(user);
            Assert.AreEqual(user, repositoryFactory.GetRepository<User>().FindEntityByID(user.UserId));
        }
              
        [Test]
        public void AddConversationEntityTest()
        {
            var conversation = new Conversation(1);
            var repositoryFactory = new RepositoryFactory();
            repositoryFactory.GetRepository<Conversation>().AddEntity(conversation);
            Assert.AreEqual(conversation, repositoryFactory.GetRepository<Conversation>().FindEntityByID(conversation.ConversationId));
        }

        [Test]
        public void AddUserEntitiesTest()
        {
            var user1 = new User("User", 1);
            var user2 = new User("User", 2);
            var users = new List<User> { user1, user2 };

            var repositoryFactory = new RepositoryFactory();

            repositoryFactory.GetRepository<User>().AddEntities(users);
            Assert.AreEqual(users, repositoryFactory.GetRepository<User>().GetAllEntities().ToList());
        }

        [Test]
        public void AddConversationEntitiesTest()
        {
            var conversation1 = new Conversation(1);
            var conversation2 = new Conversation(2);
            var conversations = new List<Conversation> { conversation1, conversation2 };

            var repositoryFactory = new RepositoryFactory();

            repositoryFactory.GetRepository<Conversation>().AddEntities(conversations);
            Assert.AreEqual(conversations, repositoryFactory.GetRepository<Conversation>().GetAllEntities().ToList());
        }

        [Test]
        public void CanNotAddSameUserEntityTwice()
        {
            var user = new User("User", 1);

            var repositoryFactory = new RepositoryFactory();
            
            repositoryFactory.GetRepository<User>().AddEntity(user);
            repositoryFactory.GetRepository<User>().AddEntity(user);

            var users = new List<User> {user};

            Assert.AreEqual(users, repositoryFactory.GetRepository<User>().GetAllEntities());
        }

        [Test] 
        public void CanNotAddSameConversationEntityTwice()
        {
            var conversation = new Conversation(1);

            var repositoryFactory = new RepositoryFactory();

            repositoryFactory.GetRepository<Conversation>().AddEntity(conversation);
            repositoryFactory.GetRepository<Conversation>().AddEntity(conversation);

            var conversations = new List<Conversation> { conversation };

            Assert.AreEqual(conversations, repositoryFactory.GetRepository<Conversation>().GetAllEntities());
        }
    }
}