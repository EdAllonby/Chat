using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Packages a <see cref="Avatar"/> without an Id for the Client to send to the Server.
    /// </summary>
    [Serializable]
    public sealed class AvatarRequest : IMessage
    {
        public AvatarRequest(int userId, Image avatar)
        {
            Contract.Requires(userId > 0);
            Contract.Requires(avatar != null);

            Avatar = new Avatar(userId, avatar);
        }

        public Avatar Avatar { get; private set; }

        public MessageIdentifier MessageIdentifier
        {
            get { return MessageIdentifier.AvatarRequest; }
        }
    }
}