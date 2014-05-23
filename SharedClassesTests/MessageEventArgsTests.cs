using NUnit.Framework;
using SharedClasses;
using SharedClasses.Message;

namespace SharedClassesTests
{
    [TestFixture]
    public class MessageEventArgsTests
    {
        [Test]
        public void MessageEventArgsTest()
        {
            const string Username = "User";
            const int UserId = 3;
            var messageEventArgs = new MessageEventArgs(new LoginRequest(Username), UserId);
            Assert.AreEqual(messageEventArgs.ClientUserId, UserId);
            Assert.IsInstanceOf<LoginRequest>(messageEventArgs.Message);
        }
    }
}