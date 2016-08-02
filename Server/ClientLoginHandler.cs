using System.Net.Sockets;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace Server
{
    /// <summary>
    /// Handles a client attempting to log in to the server.
    /// </summary>
    internal static class ClientLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientLoginHandler));

        /// <summary>
        /// From a <see cref="TcpClient" /> object, attempt to initialise a new <see cref="User" /> <see cref="IEntity" />.
        /// </summary>
        /// <param name="tcpClient">The connection between the attempting-to-connect client.</param>
        /// <param name="serviceRegistry">Holds services to initialise client</param>
        /// <returns></returns>
        public static LoginResponse InitialiseNewClient(TcpClient tcpClient, IServiceRegistry serviceRegistry)
        {
            var userRepository = (UserRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();
            var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();

            LoginRequest loginRequest = GetLoginRequest(tcpClient);
            User user = userRepository.FindUserByUsername(loginRequest.User.Username);

            LoginResponse loginResponse;

            if (IsNewUser(user))
            {
                // new user, give it unique Id and connection status of connected
                user = CreateUserEntity(loginRequest, userRepository, entityIdAllocatorFactory);
                loginResponse = new LoginResponse(user, LoginResult.Success);

                SendLoginResponse(loginResponse, tcpClient);
            }
            else if (IsExistingUser(user))
            {
                // This user already exists, just update the status of it in the repository
                userRepository.UpdateUserConnectionStatus(new ConnectionStatus(user.Id, ConnectionStatus.Status.Connected));
                loginResponse = new LoginResponse(user, LoginResult.Success);

                SendLoginResponse(loginResponse, tcpClient);
            }
            else
            {
                Log.InfoFormat($"User with user Id {user.Id} already connected, denying user login.");
                loginResponse = new LoginResponse(null, LoginResult.AlreadyConnected);
                SendLoginResponse(loginResponse, tcpClient);
            }

            return loginResponse;
        }

        private static LoginRequest GetLoginRequest(TcpClient tcpClient)
        {
            MessageIdentifier messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            IMessageSerialiser serialiser = SerialiserFactory.GetSerialiser(messageIdentifier);

            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());

            return loginRequest;
        }

        private static User CreateUserEntity(LoginRequest clientLogin, IEntityRepository<User> userRepository, EntityIdAllocatorFactory entityIdAllocator)
        {
            var newUser = new User(clientLogin.User.Username, entityIdAllocator.AllocateEntityId<User>(), new ConnectionStatus(clientLogin.User.Id, ConnectionStatus.Status.Connected));

            userRepository.AddEntity(newUser);

            return newUser;
        }

        private static void SendLoginResponse(IMessage loginResponse, TcpClient tcpClient)
        {
            IMessageSerialiser messageSerialiser = SerialiserFactory.GetSerialiser(loginResponse.MessageIdentifier);
            messageSerialiser.Serialise(tcpClient.GetStream(), loginResponse);
        }

        private static bool IsNewUser(User user)
        {
            return user == null;
        }

        private static bool IsExistingUser(User user)
        {
            return user.ConnectionStatus.UserConnectionStatus != ConnectionStatus.Status.Connected;
        }
    }
}