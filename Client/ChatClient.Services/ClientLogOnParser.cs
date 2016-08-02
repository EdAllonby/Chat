using System.Collections.Generic;
using System.Net;
using log4net;

namespace ChatClient.Services
{
    /// <summary>
    /// This class is used to Parse the IPAddress, Port and Username with validation
    /// </summary>
    public sealed class ClientLogOnParser
    {
        private const int PortMaxBound = 65535;
        private const int PortMinBound = 0;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientLogOnParser));

        private bool isParsed;

        private IPAddress targetedAddress;
        private int targetedPort;
        private string targetedUsername;

        /// <summary>
        /// Tries to parse the command line arguments to a <see cref="LoginDetails" /> object.
        /// </summary>
        /// <param name="commandLineArguments">A collection of command line arguments containing the login details.</param>
        /// <param name="loginDetails">An object to store the parsed login details.</param>
        /// <returns>Whether the parse was successful.</returns>
        public bool TryParseCommandLineArguments(IEnumerable<string> commandLineArguments, out LoginDetails loginDetails)
        {
            var parameterName = "";

            foreach (string argument in commandLineArguments)
            {
                if (argument[0] == '/')
                {
                    parameterName = argument;
                }
                else
                {
                    if (parameterName == "/Username")
                    {
                        SetUserName(argument);
                    }
                    if (parameterName == "/IPAddress" && isParsed)
                    {
                        SetIPAddress(argument);
                    }
                    if (parameterName == "/Port" && isParsed)
                    {
                        SetPort(argument);
                    }
                }
            }

            loginDetails = isParsed ? new LoginDetails(targetedUsername, targetedAddress, targetedPort) : null;

            Log.Info(isParsed
                ? "Command line arguments successfully parsed"
                : "Command line arguments incomplete. Going to manual entry of Username, Server and Port");

            return isParsed;
        }

        /// <summary>
        /// Tries to parse the username, ipAddress and port strings to a <see cref="LoginDetails" /> object.
        /// </summary>
        /// <param name="username">The username wanted to be set.</param>
        /// <param name="ipAddress">The IPAddress wanted to be set.</param>
        /// <param name="port">The port wanted to be set.</param>
        /// <param name="loginDetails">An object to store the parsed login details.</param>
        /// <returns>Whether the parse was successful.</returns>
        public bool TryParseLogonDetails(string username, string ipAddress, string port, out LoginDetails loginDetails)
        {
            SetUserName(username);

            if (isParsed)
            {
                SetIPAddress(ipAddress);
            }
            if (isParsed)
            {
                SetPort(port);
            }
            loginDetails = isParsed ? new LoginDetails(targetedUsername, targetedAddress, targetedPort) : null;

            return isParsed;
        }

        private void SetUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                isParsed = false;
            }

            targetedUsername = username;
            Log.Info("Username set as " + targetedUsername);

            isParsed = true;
        }

        private void SetIPAddress(string ipString)
        {
            IPAddress address;
            isParsed = IPAddress.TryParse(ipString, out address);

            if (isParsed)
            {
                Log.Info("User entered target IP Address " + address);
                targetedAddress = address;
            }
            else
            {
                Log.Warn("IPAddress was not a valid entry");
            }
        }

        private void SetPort(string portLine)
        {
            int port;
            isParsed = int.TryParse(portLine, out port);

            if (port > PortMaxBound || port < PortMinBound)
            {
                isParsed = false;
            }

            if (isParsed)
            {
                Log.Info("User entered port " + port);
                targetedPort = port;
            }
            else
            {
                Log.Warn("Port was not a valid entry");
            }
        }
    }
}