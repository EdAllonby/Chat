using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Server
{
    [Serializable]
    public class Client : ISerializable
    {
        private string userId;
        private Message message;
        private Status currentStatus;

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
                var clientMessage = new StringBuilder();
                clientMessage.Append(message.GetText());
                clientMessage.Append(" sent at: ");
                clientMessage.Append(message.GetTimeStamp());

                return clientMessage.ToString();
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
            DeSerialiseClient(info);
        }

        private void DeSerialiseClient(SerializationInfo info)
        {
            DeSerialiseUserId(info);
            DeSerialiseMessage(info);
            DeSerialiseCurrentStatus(info);
        }

        private void DeSerialiseUserId(SerializationInfo info)
        {
            userId = (string) info.GetValue("userId", typeof (string));
        }

        private void DeSerialiseMessage(SerializationInfo info)
        {
            message = (Message) info.GetValue("message", typeof (Message));
        }

        private void DeSerialiseCurrentStatus(SerializationInfo info)
        {
            currentStatus = (Status) info.GetValue("currentStatus", typeof (Status));
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
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