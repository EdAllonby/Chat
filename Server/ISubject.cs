namespace Server
{
    public interface ISubject
    {
        /// <summary>
        /// Subscribe an object to the subject object who wants to receive notifications of changes to the subject
        /// </summary>
        /// <param name="o">Object that wants to be notified from the subject</param>
        void RegisterObserver(IObserver o);

        /// <summary>
        /// Remove an object from the subject to no longer receive notifications
        /// </summary>
        /// <param name="o">Object that wants to be removed from the subscription list</param>
        void RemoveObserver(IObserver o);

        /// <summary>
        /// Notify all subscribed observers of changes to the subject
        /// </summary>
        void NotifyObserversOfClientChange();
    }
}