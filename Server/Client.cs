using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Server
{
    [Serializable]
    public class Client : ISerializable
    {
        private readonly string userId;
        private Message message;
        private readonly Status currentStatus;

        // If Client is created
        public Client(string userId)
        {
            currentStatus = Status.Connected;
            this.userId = userId;
        }

        public Client(string userId, Message message, Status currentStatus)
        {
            this.userId = userId;
            this.message = message;
            this.currentStatus = currentStatus;
        }

        public string GetMessage()
        {
            if (message != null)
            {
                string clientMessage = message.GetText() + message.GetTimeStamp();
                return clientMessage;

            }
            return null;
        }

        public string GetUserId()
        {
            return userId;
        }

        public Status GetStatus()
        {
            return currentStatus;
        }

        public Client(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            userId = (string) info.GetValue("userId", typeof (string));
            message = (Message) info.GetValue("message", typeof (Message));
            currentStatus = (Status) info.GetValue("currentStatus", typeof (Status));
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("UserId", userId);
            info.AddValue("message", message);
            info.AddValue("currentStatus", currentStatus);
        }

        public void NewMessage(string messageBody)
        {
            DateTime messageTime = DateTime.Now;
            var newMessage = new Message(messageBody, messageTime);
            message = newMessage;
        }
    }
}