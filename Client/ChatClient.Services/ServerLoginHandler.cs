using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatClient.Services.MessageHandler;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace ChatClient.Services
{
    /// <summary>
    /// Creates a connection to the Server.
    /// </summary>
    internal sealed class ServerLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerLoginHandler));

        private readonly TcpClient serverConnection = new TcpClient();
        private bool hasReceivedConversationSnapshot;
        private bool hasReceivedParticipationSnapshot;

        private bool hasReceivedUserSnapshot;

        /// <summary>
        /// Initialises a server login helper.
        /// </summary>
        public ServerLoginHandler(MessageHandlerRegistry messageHandlerRegistry)
        {
            foreach (IBootstrapper bootstrapper in messageHandlerRegistry.Bootstrappers)
            {
                bootstrapper.EntityBootstrapCompleted += EntityBootstrapCompleted;
            }
        }

        /// <summary>
        /// Fires when bootstrapping to all repositories has completed.
        /// </summary>
        public event EventHandler BootstrapCompleted;

        /// <summary>
        /// Handles connecting a client to the server, and creates a <see cref="ConnectionHandler" /> on success.
        /// </summary>
        /// <param name="loginDetails">The connection details of the trying-to-connect user.</param>
        /// <param name="connectionHandler">An initialised connection handler on login success.</param>
        /// <returns></returns>
        public LoginResponse ConnectToServer(LoginDetails loginDetails, out ConnectionHandler connectionHandler)
        {
            bool isServerFound = CreateConnection(loginDetails.Address, loginDetails.Port);

            if (!isServerFound)
            {
                connectionHandler = null;
                return new LoginResponse(LoginResult.ServerNotFound);
            }

            IMessage userRequest = new LoginRequest(loginDetails.Username);
            SendConnectionMessage(userRequest);
            var loginResponse = (LoginResponse) GetConnectionIMessage();

            if (loginResponse.LoginResult == LoginResult.Success)
            {
                BootstrapRepositories(loginResponse.User.Id);

                connectionHandler = new ConnectionHandler(loginResponse.User.Id, serverConnection);

                Log.DebugFormat("Connection process to the server has finished");
            }
            else
            {
                connectionHandler = null;
            }

            return loginResponse;
        }

        private void BootstrapRepositories(int userId)
        {
            SendConnectionMessage(new EntitySnapshotRequest<User>(userId));
            SendConnectionMessage(new EntitySnapshotRequest<Conversation>(userId));
            SendConnectionMessage(new EntitySnapshotRequest<Participation>(userId));
        }

        private void EntityBootstrapCompleted(object sender, EntityBootstrapEventArgs e)
        {
            if (e.EntityType == typeof(User))
            {
                hasReceivedUserSnapshot = true;
            }
            else if (e.EntityType == typeof(Conversation))
            {
                hasReceivedConversationSnapshot = true;
            }
            else if (e.EntityType == typeof(Participation))
            {
                hasReceivedParticipationSnapshot = true;
            }
            else
            {
                string errorMessage = $"{typeof(ServerLoginHandler).Name} class should not be bootstrapping for an entity of type {e.EntityType.Name}";
                Log.ErrorFormat(errorMessage);

                throw new ArgumentException(errorMessage);
            }

            TrySendBootstrapCompleteEvent();
        }

        private bool CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("ClientService looking for server with address: " + targetAddress + ":" + targetPort);

            serverConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = serverConnection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), false))
                {
                    serverConnection.Close();
                    Log.Warn("Timed out trying to find server.");
                    return false;
                }

                serverConnection.EndConnect(asyncResult);
                Log.Info("ClientService found server, connection created");

                return true;
            }
            catch (SocketException)
            {
                Log.Info("Port value is incorrect.");
                return false;
            }
            finally
            {
                waitHandle.Close();
            }
        }

        private void TrySendBootstrapCompleteEvent()
        {
            if (HasReceivedAllBootstraps())
            {
                Log.Debug("Client bootstrap complete. Sending Bootstrap Completed event.");
                OnBootstrapCompleted();
            }
        }

        private bool HasReceivedAllBootstraps()
        {
            return hasReceivedUserSnapshot &&
                   hasReceivedConversationSnapshot &&
                   hasReceivedParticipationSnapshot;
        }

        private IMessage GetConnectionIMessage()
        {
            MessageIdentifier messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(serverConnection.GetStream());

            IMessageSerialiser serialiser = SerialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(serverConnection.GetStream());
        }

        private void SendConnectionMessage(IMessage message)
        {
            IMessageSerialiser messageSerialiser = SerialiserFactory.GetSerialiser(message.MessageIdentifier);
            messageSerialiser.Serialise(serverConnection.GetStream(), message);
        }

        private void OnBootstrapCompleted()
        {
            EventHandler handler = BootstrapCompleted;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}