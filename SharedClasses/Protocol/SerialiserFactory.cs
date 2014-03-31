namespace SharedClasses.Protocol
{
    /// <summary>
    /// Class to get the correct serialiser for the given <see cref="IMessage"/> identifier
    /// </summary>
    public class SerialiserFactory
    {
        /// <summary>
        /// Returns the correct serialiser for the <see cref="IMessage"/> object identifier
        /// defined in the <see cref="MessageUtilities"/> class
        /// </summary>
        /// <param name="messageIdentifier">The type of message that will be sent</param>
        /// <returns>The serialiser used to serialise and deserialise the message</returns>
        public ISerialiser GetSerialiser(int messageIdentifier)
        {
            ISerialiser serialiser = null;
            switch (messageIdentifier)
            {
                case 1:
                    serialiser = new ContributionRequestSerialiser();
                    break;
                case 2:
                    serialiser = new ContributionNotificationSerialiser();
                    break;
                case 3:
                    serialiser = new LoginRequestSerialiser();
                    break;
            }

            return serialiser;
        }
    }
}