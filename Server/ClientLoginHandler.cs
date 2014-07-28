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

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();
        private readonly UserRepository userRepository;

        public ClientLoginHandler(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public LoginResponse InitialiseNewClient(TcpClient tcpClient, EntityIdAllocatorFactory entityIdAllocator)
        {
            LoginRequest loginRequest = GetLoginRequest(tcpClient);
            User user = userRepository.FindUserByUsername(loginRequest.User.Username);

            LoginResponse loginResponse;

            if (user == null || user.ConnectionStatus.UserConnectionStatus != ConnectionStatus.Status.Connected)
            {
                if (user == null)
                {
                    // new user, give it unique ID and connection status of connected
                    user = CreateUserEntity(loginRequest, entityIdAllocator);
                }
                else
                {
                    // This user already exists, just update the status of it in the repository
                    userRepository.UpdateUserConnectionStatus(new ConnectionStatus(user.Id, ConnectionStatus.Status.Connected));
                }

                loginResponse = new LoginResponse(user, LoginResult.Success);

                SendConnectionMessage(loginResponse, tcpClient);
            }
            else
            {
                Log.InfoFormat("User with user Id {0} already connected, denying user login.", user.Id);
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

        private User CreateUserEntity(LoginRequest clientLogin, EntityIdAllocatorFactory entityIdAllocator)
        {
            var newUser = new User(clientLogin.User.Username, entityIdAllocator.AllocateEntityId<User>(), new ConnectionStatus(clientLogin.User.Id, ConnectionStatus.Status.Connected));

            userRepository.AddEntity(newUser);

            return newUser;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(tcpClient.GetStream(), message);
        }
    }
}