using NUnit.Framework;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace SharedClassesTests
{
    [TestFixture]
    public class SerialiserFactoryTests
    {
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        [Test]
        public void GetSerialiserFromGenericTest()
        {
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ParticipationsNotification>();
            Assert.IsInstanceOf<ParticipationsNotificationSerialiser>(serialiser);
        }

        [Test]
        public void GetSerialiserFromIMessageTest()
        {
            IMessage message = new LoginRequest("User");

            ISerialiser serialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            Assert.IsInstanceOf<LoginRequestSerialiser>(serialiser);
        }
    }
}