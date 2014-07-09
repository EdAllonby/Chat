﻿using System.Net.Sockets;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="LoginRequest" /> object
    /// Uses <see cref="UserSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class LoginRequestSerialiser : Serialiser<LoginRequest>
    {
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();
        private readonly UserSerialiser userSerialiser = new UserSerialiser();

        protected override void Serialise(LoginRequest message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.Serialise(networkStream, message.MessageIdentifier);

            userSerialiser.Serialise(networkStream, message.User);
            Log.InfoFormat("{0} message serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            User user = userSerialiser.Deserialise(networkStream);
            var loginRequest = new LoginRequest(user.Username);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", loginRequest.MessageIdentifier);
            return loginRequest;
        }
    }
}