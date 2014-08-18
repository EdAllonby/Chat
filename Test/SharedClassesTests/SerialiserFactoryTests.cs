using NUnit.Framework;
using SharedClasses.Message;
using SharedClasses.Serialiser;
using SharedClasses.Serialiser.MessageSerialiser;

namespace SharedClassesTests
{
    [TestFixture]
    public class SerialiserFactoryTests
    {
        [Test]
        public void GetSerialiserFromGenericTest()
        {
            ISerialiser serialiser = SerialiserFactory.GetSerialiser<ParticipationNotification>();
            Assert.IsInstanceOf<ParticipationNotificationSerialiser>(serialiser);
        }

        [Test]
        public void GetSerialiserFromIMessageTest()
        {
            IMessage message = new LoginRequest("User");

            ISerialiser serialiser = SerialiserFactory.GetSerialiser(message.MessageIdentifier);
            Assert.IsInstanceOf<LoginRequestSerialiser>(serialiser);
        }
    }
}