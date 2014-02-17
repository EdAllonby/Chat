using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;

namespace Server
{
    public static class Program
    {
        private static void Main()
        {
            RunTestClient();
        }

        private static void RunTestClient()
        {
            // Create a a ServerData object (which will be used eventually to accept TCP connection)
            var serverData = new ServerData(new XmlFormat());

            // Create a ServerLog object
            var log = new ServerLog(serverData);

            Console.ReadKey();
        }
    }
}