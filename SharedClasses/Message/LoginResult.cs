namespace SharedClasses.Message
{
    /// <summary>
    /// The outcome of the client's login request, whether they are allowed on to the Server.
    /// </summary>
    public enum LoginResult
    {
        Success,
        AlreadyConnected,
        ServerNotFound
    }
}