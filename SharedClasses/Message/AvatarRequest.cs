using System;
using System.Drawing;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Avatar" /> without an Id for the Client to send to the Server.
    /// </summary>
    [Serializable]
    public sealed class AvatarRequest : IMessage
    {
        public AvatarRequest(int userId, Image avatar)
        {
            Avatar = new Avatar(userId, avatar);
        }

        public Avatar Avatar { get; private set; }

        public MessageIdentifier MessageIdentifier => MessageIdentifier.AvatarRequest;
    }
}