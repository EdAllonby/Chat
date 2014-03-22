using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;

namespace Client
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));
        private static IPAddress targetedAddress;
        private static int targetedPort;
        private static IEnumerable<string> commandLineArguments; 

        private static void Main(string[] args)
        {
            commandLineArguments = args;
            SetUserLogonMethod();
            Thread.CurrentThread.Name = "Main Thread";
            var MainClient = new Client(targetedAddress, targetedPort);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void SetUserLogonMethod()
        {
            Console.WriteLine("Press 1 to connect the client on a local environment, or 2 to connect the client to an address");
            string usersConnectionChoice = Console.ReadLine();

            int connectionChoice;
            bool connectionChoiceResult = int.TryParse(usersConnectionChoice, out connectionChoice);

            if (connectionChoiceResult)
            {
                switch (connectionChoice)
                {
                    case 1:
                        targetedAddress = LocalIPAddress();
                        targetedPort = 5004;
                        Log.Debug("Port set to 5004");
                        break;
                    case 2:
                        if (!commandLineArguments.Any())
                        {
                            Console.Write("Enter IP Address (e.g. 127.0.0.1): ");
                            string ipString = Console.ReadLine();
                            SetIPAddress(ipString);

                            Console.Write("Enter a port: ");
                            string port = Console.ReadLine();
                            SetPort(port);
                        }
                        else
                        {
                            ParseCommandLineArguments();
                        }
                        break;
                }
            }
        }

        private static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            Log.Debug("Local IP Address found: " + host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork));
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        private static void ParseCommandLineArguments()
        {
            string parameterName = "";

            foreach (var argument in commandLineArguments)
            {
                if (argument[0] == '/')
                {
                    parameterName = argument;
                }
                else
                {
                    if (parameterName == "/IPAddress")
                    {
                        SetIPAddress(argument);
                    }
                    if (parameterName == "/Port")
                    {
                        SetPort(argument);
                    }
                }
            }
        }

        private static void SetIPAddress(string ipString)
        {
            IPAddress address;
            bool addressResult = IPAddress.TryParse(ipString, out address);
            if (addressResult == false)
            {
                Console.WriteLine("You didn't enter a valid IP, setting target IP Address to 127.0.0.1");

                address = IPAddress.Parse("127.0.0.1");
                Log.Warn(address + " was not a valid entry, setting target IP Address to 127.0.0.1");
            }
            else
            {
                Log.Info("User entered target IP Address " + address);
            }

            targetedAddress = address;
        }

        private static void SetPort(string portLine)
        {
            int port;
            bool portResult = int.TryParse(portLine, out port);
            if (portResult == false)
            {
                Console.WriteLine("You didn't enter a number, setting port as 5004");
                port = 5004;
                Log.Warn(port + "was not a valid entry, setting port to default of 5004");
            }
            else
            {
                Log.Info("User entered port " + port);
            }
            targetedPort = port;
        }
    }
}