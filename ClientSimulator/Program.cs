using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChatClient.Services;
using ClientSimulator.Properties;
using SharedClasses;
using SharedClasses.Domain;

namespace ClientSimulator
{
    /// <summary>
    /// Bombard the Server with generated clients. Mainly for concurrency and stress testing.
    /// </summary>
    internal static class Program
    {
        private const int TotalClients = 10;
        private static readonly List<ClientService> Clients = new List<ClientService>();
        private static bool areRepositoriesValid;

        private static void Main()
        {
            for (int i = 0; i < TotalClients; i++)
            {
                RepositoryManager repositoryManager = new RepositoryManager();
                repositoryManager.AddRepository<User>(new UserRepository());
                repositoryManager.AddRepository<Conversation>(new ConversationRepository());
                repositoryManager.AddRepository<Participation>(new ParticipationRepository());

                Clients.Add(new ClientService(repositoryManager));
            }

            LoginClients();

            if (areRepositoriesValid)
            {
                StartMultiUserConversation();

                foreach (Thread thread in Clients.Select(clientService => new Thread(() => SendContributions(clientService))))
                {
                    Thread.Sleep(1000);

                    thread.Start();
                }

                Console.ReadKey();
            }
        }

        private static void LoginClients()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            Parallel.For(0, TotalClients, client => Clients[client].LogOn(new LoginDetails(String.Format("user{0}", client), ipAddress, 5004)));

            // Make sure all clients have finished running their initialisation threads.
            Thread.Sleep(1000);
            areRepositoriesValid = ClientRepositoryValidator.ValidateUserRepository(Clients);
            Thread.Sleep(1000);
        }

        private static void StartMultiUserConversation()
        {
            var participants = new List<int>();
            for (int i = 1; i <= TotalClients; i++)
            {
                participants.Add(i);
            }

            Clients[0].CreateConversation(participants);
        }

        private static void SendContributions(IClientService client)
        {
            while (true)
            {
                client.SendContribution(1, "hello");
            }
        }

        private static void SendAvatar(IClientService client)
        {
            client.SendAvatarRequest(Resources.LargeImage);
        }
    }
}