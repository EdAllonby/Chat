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

        public Client()
        {
            
        }

        // If Client is created
        public Client(string userId)
        {
            currentStatus = Status.Connected;
            this.userId = userId;
            message = null;
        }

        // If client has a message, then its status = NewMessage
        public Client(string userId, string message)
        {
            this.userId = userId;
            this.message = new Message(message);
            currentStatus = Status.NewMessage;
        }

        public string GetMessage()
        {
            if (message != null)
            {
                string clientMessage = message.GetText() + " sent at: " + message.GetTimeStamp();
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

        protected Client(SerializationInfo info, StreamingContext context)
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

            info.AddValue("userId", userId);
            info.AddValue("message", message);
            info.AddValue("currentStatus", currentStatus);
        }
    }
}