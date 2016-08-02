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
        public void GetSerialiserFromIMessageTest()
        {
            IMessage message = new LoginRequest("User");

            IMessageSerialiser serialiser = SerialiserFactory.GetSerialiser(message.MessageIdentifier);
            Assert.IsInstanceOf<MessageSerialiser<LoginRequest>>(serialiser);
        }
    }
}