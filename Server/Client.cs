using System.Text;

namespace Server
{
    public class Client
    {
        private string userID;
        private Message message;
        private CurrentStatus currentStatus;

        public string GetMessage()
        {
            string clientMessage = message.GetText() + message.GetTimeStamp();
            return clientMessage;
        }

        public string GetUserId()
        {
            return userID;
        }

        public CurrentStatus GetStatus()
        {
            return currentStatus;
        }
    }
}