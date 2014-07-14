using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise an <see cref="AvatarRequest" /> message.
    /// </summary>
    internal sealed class AvatarRequestSerialiser : Serialiser<AvatarRequest>
    {
        private readonly AvatarSerialiser avatarSerialiser = new AvatarSerialiser();

        protected override void Serialise(NetworkStream networkStream, AvatarRequest avatarRequest)
        {
            avatarSerialiser.Serialise(networkStream, avatarRequest.Avatar);
            Log.InfoFormat("{0} message serialised.", avatarRequest.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            Log.Debug("Waiting for avatar request message to deserialise");
            Avatar avatar = avatarSerialiser.Deserialise(networkStream);

            var avatarRequest = new AvatarRequest(avatar.UserId, avatar.UserAvatar);

            Log.Info("Avatar request message deserialised");

            return avatarRequest;
        }
    }
}