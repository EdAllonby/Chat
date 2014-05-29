using System.Net.Sockets;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace Server
{
    internal sealed class ClientLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientLoginHandler));

        private readonly EntityGeneratorFactory entityIDGenerator;
        private readonly RepositoryManager repositoryManager;

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public ClientLoginHandler(EntityGeneratorFactory entityIDGenerator, RepositoryManager repositoryManager)
        {
            this.entityIDGenerator = entityIDGenerator;
            this.repositoryManager = repositoryManager;
        }

        public LoginResponse InitialiseNewClient(TcpClient tcpClient)
        {
            LoginRequest loginRequest = GetLoginRequest(tcpClient);
            User user = repositoryManager.UserRepository.FindUserByUsername(loginRequest.User.Username);

            LoginResponse loginResponse;

            if (user == null || user.ConnectionStatus != ConnectionStatus.Connected)
            {
                if (user == null)
                {
                    // new user, give it unique ID and connection status of connected
                    user = CreateUserEntity(loginRequest);
                }
                else
                {
                    // This user already exists, just update the status of it in the repository
                    user.ConnectionStatus = ConnectionStatus.Connected;
                    repositoryManager.UserRepository.UpdateUser(user);
                }

                loginResponse = new LoginResponse(user, LoginResult.Success);

                SendConnectionMessage(loginResponse, tcpClient);
            }
            else
            {
                Log.InfoFormat("User with user Id {0} already connected, denying user login.", user.UserId);
                loginResponse = new LoginResponse(null, LoginResult.AlreadyConnected);
                SendConnectionMessage(loginResponse, tcpClient);
            }
         
            return loginResponse;
        }

        private LoginRequest GetLoginRequest(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());

            return loginRequest;
        }

        private User CreateUserEntity(LoginRequest clientLogin)
        {
            var newUser = new User(clientLogin.User.Username, entityIDGenerator.GetEntityID<User>(), ConnectionStatus.Connected);

            repositoryManager.UserRepository.UpdateUser(newUser);

            return newUser;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }
    }
}