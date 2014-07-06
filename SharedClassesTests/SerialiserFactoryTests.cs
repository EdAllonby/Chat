using NUnit.Framework;
using SharedClasses.Message;
using SharedClasses.Serialiser;
using SharedClasses.Serialiser.MessageSerialiser;

namespace SharedClassesTests
{
    [TestFixture]
    public class SerialiserFactoryTests
    {
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        [Test]
        public void GetSerialiserFromGenericTest()
        {
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ParticipationNotification>();
            Assert.IsInstanceOf<ParticipationNotificationSerialiser>(serialiser);
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