using System.Text;

namespace Server
{
    public class Client
    {
        private string userID;
        private Message message;
        private Status currentStatus;

        public Client(string userId, Message message, Status currentStatus)
        {
            userID = userId;
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
            return userID;
        }

        public Status GetStatus()
        {
            return currentStatus;
        }
    }
}