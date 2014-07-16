using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChatClient.Services;
using SharedClasses.Domain;

namespace ClientSimulator
{
    /// <summary>
    /// Bombard the Server with generated clients. Mainly for concurrency and stress testing.
    /// </summary>
    internal static class Program
    {
        private static readonly List<IClientService> Clients = new List<IClientService>();
        private const int TotalClients = 30;

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
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            Parallel.For(0, TotalClients, client => Clients[client].LogOn(new LoginDetails(String.Format("user{0}", client), ipAddress, 5004)));

            // Make sure all clients have finished running their initialisation threads.
            Thread.Sleep(1000);

            ClientRepositoryValidator.ValidateUserRepository(Clients);
        }
    }
}