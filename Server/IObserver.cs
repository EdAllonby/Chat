namespace Server
{
    public interface IObserver
    {
        /// <summary>
        /// When the Subject has something to push out to the subscribed observers, call this method
        /// </summary>
        /// <param name="client">A client object will be passed to the observer</param>
        void Update(Client client);
    }
}