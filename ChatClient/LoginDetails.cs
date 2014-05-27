using System;
using System.Net;

namespace ChatClient
{
    /// <summary>
    /// Holds login information
    /// </summary>
    public class LoginDetails
    {
        private readonly String username;
        private readonly IPAddress address;
        private readonly int port;

        public LoginDetails(string username, IPAddress address, int port)
        {
            this.username = username;
            this.address = address;
            this.port = port;
        }

        /// <summary>
        /// The username used to login
        /// </summary>
        public string Username
        {
            get { return username; }
        }

        /// <summary>
        /// The IP targeted Address used to login
        /// </summary>
        public IPAddress Address
        {
            get { return address; }
        }

        /// <summary>
        /// The targeted Port used to login
        /// </summary>
        public int Port
        {
            get { return port; }
        }
    }
}