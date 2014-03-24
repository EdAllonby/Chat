using System;
using System.Net.Sockets;
using log4net;
using SharedClasses.Protocol;

namespace SharedClasses.Domain
{
    [Serializable]
    public class Contribution
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Contribution));

        private static readonly ContributionSerialiser SerialiseMessage = new ContributionSerialiser();

        private string text;
        private DateTime messageTimeStamp;

        public Contribution(string text)
        {
            CreateMessage(text);
            Log.Debug("Contribution created");
        }

        public string GetMessage()
        {
            return text + " @ " + messageTimeStamp;
        }

        private void CreateMessage(string messageText)
        {
            SetTextOfMessage(messageText ?? String.Empty);
            SetTimeStampOfMessage(); 
        }

        public void Serialise(NetworkStream networkStream)
        {
            SerialiseMessage.Serialise(networkStream, this);
        }

        public static Contribution Deserialise(NetworkStream networkStream)
        {
            return SerialiseMessage.Deserialise(networkStream);
        }

        private void SetTextOfMessage(string messageText)
        {
            text = messageText;
            Log.Debug("Contribution text set: " + text);
        }

        private void SetTimeStampOfMessage()
        {
            messageTimeStamp = DateTime.Now;
            Log.Debug("Time stamp created: " + messageTimeStamp);
        }
    }
}