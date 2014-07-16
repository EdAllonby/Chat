using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChatClient.Services;
using SharedClasses.Domain;

namespace ClientSimulator
{
    /// <summary>
    /// Used to validate contents stored in repositories.
    /// </summary>
    internal class ClientRepositoryValidator
    {
        public static void ValidateUserRepository(IReadOnlyCollection<IClientService> clients)
        {
            int totalClients = clients.Count;

            int userIdSum = 0;

            for (int id = 0; id <= totalClients; id++)
            {
                userIdSum += id;
            }

            int sum = clients.Sum(client => client.ClientUserId);

            Debug.Assert(sum == userIdSum);

            foreach (UserRepository userRepository in clients.Select(client => client.RepositoryManager.UserRepository))
            {
                sum = userRepository.GetAllUsers().Sum(user => user.UserId);
                Debug.Assert(sum == userIdSum);
            }
        }
    }
}