using System;
using System.Collections.Generic;
using System.Net;
using log4net;

namespace ChatClient
{
    /// <summary>
    ///     This class is used to Parse the IPAddress, Port and Username with validation
    /// </summary>
    internal class ClientLoginParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientLoginParser));

        public String Username { get; private set; }
        public IPAddress TargetedAddress { get; private set; }
        public int TargetedPort { get; private set; }

        public bool ParseCommandLineArguments(IEnumerable<String> commandLineArguments)
        {
            string parameterName = "";
            bool result = false;
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
                        result = SetUserName(argument);
                    }
                    if (parameterName == "/IPAddress" && result)
                    {
                        result = SetIPAddress(argument);
                    }
                    if (parameterName == "/Port" && result)
                    {
                        result = SetPort(argument);
                    }
                }
            }
            Log.Info(result
                ? "Command line arguments successfully parsed"
                : "Command line arguments incomplete. Going to manual entry of Username, Server and Port");

            return result;
        }

        public bool ParseLogonDetails(string username, string ipAddress, string port)
        {
            bool result = SetUserName(username);
            if (result)
            {
                result = SetIPAddress(ipAddress);
            }
            if (result)
            {
                result = SetPort(port);
            }

            return result;
        }

        private bool SetUserName(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return false;
            }

            Username = userName;
            Log.Info("Username set as " + Username);

            return true;
        }

        private bool SetIPAddress(string ipString)
        {
            IPAddress address;
            bool addressResult = IPAddress.TryParse(ipString, out address);

            if (addressResult)
            {
                Log.Info("User entered target IP Address " + address);
                TargetedAddress = address;
            }
            else
            {
                Log.Warn(address + " was not a valid entry");
            }

            return addressResult;
        }

        private bool SetPort(string portLine)
        {
            int port;
            bool portResult = int.TryParse(portLine, out port);
            if (portResult)
            {
                Log.Info("User entered port " + port);
                TargetedPort = port;
            }
            else
            {
                port = 5004;
                Log.Warn(port + " was not a valid entry");
            }

            return portResult;
        }
    }
}