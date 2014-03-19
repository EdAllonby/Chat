using System;
using System.Net;
using System.Threading;
using log4net;

namespace Client
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));
        private static IPAddress targetedAddress;
        private static int targetedPort;

        private static void Main(string[] args)
        {


            if (args.Length == 0)
            {
                Console.Write("Enter IP Address (e.g. 127.0.0.1): ");
                string ipString = Console.ReadLine();
                targetedAddress = SetIPAddress(ipString);

                Console.Write("Enter a port: ");
                string port = Console.ReadLine();
                targetedPort = SetPort(port);
            }
            else
            {
                ParseCommandLineArguments(args);
            }

            Thread.CurrentThread.Name = "Main Thread";
            var MainClient = new Client(targetedAddress, targetedPort);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            string parameterName = "";

            foreach (var argument in args)
            {
                if (argument[0] == '/')
                {
                    parameterName = argument;
                }
                else
                {
                    if (parameterName == "/IPAddress")
                    {
                        targetedAddress = SetIPAddress(argument);
                    }
                    if (parameterName == "/Port")
                    {
                        targetedPort = SetPort(argument);
                    }
                }
            }
        }

        private static int SetPort(string portLine)
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
            return port;
        }

        private static IPAddress SetIPAddress(string ipString)
        {

            IPAddress address;
            bool addressResult = IPAddress.TryParse(ipString, out address);
            if (addressResult == false)
            {
                Console.WriteLine("You didn't enter a valid IP, setting target IPAdress to 127.0.0.1");

                address = IPAddress.Parse("127.0.0.1");
                Log.Warn(address + " was not a valid entry, setting target IPAdress to 127.0.0.1");
            }
            else
            {
                Log.Info("User entered target IPAddress " + address);
            }
            return address;
        }
    }
}