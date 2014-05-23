using NUnit.Framework;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClassesTests
{
    [TestFixture]
    public class SerialiserFactoryTests
    {
        private SerialiserFactory serialiserFactory = new SerialiserFactory();

        [Test]
        public void GetSerialiserFromIMessageTest()
        {
            IMessage message = new LoginRequest("User");

            ISerialiser serialiser = serialiserFactory.GetSerialiser(message.Identifier);
            Assert.IsInstanceOf<LoginRequestSerialiser>(serialiser);
        }

        [Test]
        public void GetSerialiserFromGenericTest()
        {
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ConversationNotification>();
            Assert.IsInstanceOf<ConversationNotificationSerialiser>(serialiser);
        }
    }
}