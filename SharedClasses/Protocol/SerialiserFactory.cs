namespace SharedClasses.Protocol
{
    public class SerialiserFactory
    {
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