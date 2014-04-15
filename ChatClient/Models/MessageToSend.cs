namespace ChatClient.Models
{
    public class MessageToSend
    {
        private readonly Client client = Client.GetInstance();

        public string Text;

        public MessageToSend(string message)
        {
            client.SendContributionRequestMessage(message);
        }
    }
}