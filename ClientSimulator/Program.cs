using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ChatClient.Services;

namespace ClientSimulator
{
    /// <summary>
    /// Bombard the Server with generated clients. Mainly for concurrency and stress testing.
    /// </summary>
    internal static class Program
    {
        private static readonly List<IClientService> Clients = new List<IClientService>();
        const int TotalClients = 100;

        private static void Main()
        {
            for (int i = 0; i < TotalClients; i++)
            {
                Clients.Add(new ClientService());
            }

            LoginClients();
            Console.ReadKey();
        }

        private static void LoginClients()
        {
            IPAddress ipAddress = IPAddress.Parse("46.33.28.2");

            Parallel.For(0, TotalClients, client => Clients[client].LogOn(new LoginDetails(String.Format("user{0}", client), ipAddress, 5004)));
        }
    }
}