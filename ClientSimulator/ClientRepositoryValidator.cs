using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChatClient.Services;
using log4net;
using SharedClasses.Domain;

namespace ClientSimulator
{
    /// <summary>
    /// Used to validate contents stored in repositories.
    /// </summary>
    internal static class ClientRepositoryValidator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientRepositoryValidator));

        public static bool ValidateUserRepository(IReadOnlyCollection<IClientService> clients)
        {
            bool areRepositoriesValid = true;

            Log.Info("Starting User Repository Validator");
            Log.Info("===================================");
            int totalClients = clients.Count;

            int userIdSum = 0;

            for (int id = 0; id <= totalClients; id++)
            {
                userIdSum += id;
                Log.InfoFormat("Total client count: {0}", totalClients);
            }

            int sum = clients.Sum(client => client.ClientUserId);

            Debug.Assert(sum == userIdSum);

            foreach (UserRepository userRepository in clients.Select(client => client.RepositoryManager.UserRepository))
            {
                int userCount = userRepository.GetAllEntities().Count();

                Log.InfoFormat("Total users in repository count: {0}", userCount);

                if (userCount != totalClients)
                {
                    Log.Error("User Repository invalid");
                    areRepositoriesValid = false;
                    break;
                }
            }
            return areRepositoriesValid;
        }
    }
}