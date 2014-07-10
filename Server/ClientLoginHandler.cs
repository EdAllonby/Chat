using System.Net.Sockets;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;
using SharedClasses.Serialiser.MessageSerialiser;

namespace Server
{
    internal sealed class ClientLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientLoginHandler));

        private readonly RepositoryManager repositoryManager;

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        public ClientLoginHandler(RepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        public LoginResponse InitialiseNewClient(TcpClient tcpClient, EntityGeneratorFactory entityGenerator)
        {
            LoginRequest loginRequest = GetLoginRequest(tcpClient);
            User user = repositoryManager.UserRepository.FindUserByUsername(loginRequest.User.Username);

            LoginResponse loginResponse;

            if (user == null || user.ConnectionStatus != ConnectionStatus.Connected)
            {
                if (user == null)
                {
                    // new user, give it unique ID and connection status of connected
                    user = CreateUserEntity(loginRequest, entityGenerator);
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

            MessageIdentifier messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            var loginRequest = (LoginRequest) serialiser.Deserialise(tcpClient.GetStream());

            return loginRequest;
        }

        private User CreateUserEntity(LoginRequest clientLogin, EntityGeneratorFactory entityGenerator)
        {
            var newUser = new User(clientLogin.User.Username, entityGenerator.GetEntityID<User>(), ConnectionStatus.Connected);

            repositoryManager.UserRepository.UpdateUser(newUser);

            return newUser;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(tcpClient.GetStream(), message);
        }
    }
}