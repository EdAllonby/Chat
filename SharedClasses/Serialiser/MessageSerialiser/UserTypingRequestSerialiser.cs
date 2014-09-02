using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    public sealed class UserTypingRequestSerialiser : Serialiser<UserTypingRequest>
    {
        private readonly UserTypingSerialiser userTypingSerialiser = new UserTypingSerialiser();


        protected override void Serialise(NetworkStream networkStream, UserTypingRequest message)
        {
            userTypingSerialiser.Serialise(networkStream, message.UserTyping);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var userTypingRequest = new UserTypingRequest(userTypingSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", userTypingRequest.MessageIdentifier);
            return userTypingRequest;
        }
    }
}