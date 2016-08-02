namespace SharedClasses.Message
{
    /// <summary>
    /// The outcome of the client's login request, whether they are allowed on to the Server.
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// The client has successfully logged in to the server.
        /// </summary>
        Success,

        /// <summary>
        /// The client cannot login because it is already connected with the same credentials.
        /// </summary>
        AlreadyConnected,

        /// <summary>
        /// The client cannot login because it cannot reach the server to connect to.
        /// </summary>
        ServerNotFound
    }
}