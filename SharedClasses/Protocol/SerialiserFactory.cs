namespace SharedClasses.Protocol
{
    public class SerialiserFactory
    {
        public ISerialise GetSerialiser(int messageIdentity)
        {
            ISerialise serialiser = null;
            switch (messageIdentity)
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