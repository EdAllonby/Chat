using System.Net;

namespace ChatClient.Services
{
    /// <summary>
    /// Holds login information
    /// </summary>
    public class LoginDetails
    {
        public LoginDetails(string username, IPAddress address, int port)
        {
            Username = username;
            Address = address;
            Port = port;
        }

        /// <summary>
        /// The username used to login
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The IP targeted Address used to login
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// The targeted Port used to login
        /// </summary>
        public int Port { get; }
    }
}