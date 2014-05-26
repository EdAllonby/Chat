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
            var messageEventArgs = new MessageEventArgs(new LoginRequest(Username));
            Assert.IsInstanceOf<LoginRequest>(messageEventArgs.Message);
        }
    }
}