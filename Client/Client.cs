using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Protocol;

namespace Client
{
    internal class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Client));

        private readonly MessageReceiver messageReceiver = new MessageReceiver();
        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;
        private readonly string userName;
        private TcpClient connection;
        private NetworkStream stream;

        private readonly List<User> connectedUsers = new List<User>(); 

        public Client(IPAddress targetAddress, int targetPort)
        {
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;

            Console.Write("Enter name: ");
            userName = Console.ReadLine();

            try
            {
                ConnectToServer();

                while (true)
                {
                    SendContributionRequestMessage();
                }
            }
            catch (SocketException socketException)
            {
                Log.Error("Could not connect to the server, exiting...", socketException);
            }
            catch (IOException ioException)
            {
                Log.Error("could not send data to the server, connection lost.", ioException);
            }
            finally
            {
                //close the client and stream
                if (stream != null)
                {
                    stream.Close();
                    Log.Info("Stream closed");
                }
                if (connection != null)
                {
                    connection.Close();
                    Log.Info("Client connection closed");
                }
            }
        }

        private void ConnectToServer()
        {
            Log.Info("Client looking for server");
            var client = new TcpClient(targetAddress.ToString(), targetPort);
            Log.Info("Client found server, connection created");

            stream = client.GetStream();
            Log.Info("Created stream with Server");

            var messageListenerThread = new Thread(ReceiveMessageListener)
            {
                Name = "MessageListenerThread"
            };
            messageListenerThread.Start();
            connection = client;

            SendLoginRequest();
        }

        private void SendLoginRequest()
        {
            ISerialiser loginRequestSerialiser = serialiserFactory.GetSerialiser<LoginRequest>();
            var loginRequest = new LoginRequest(userName);
            loginRequestSerialiser.Serialise(loginRequest, stream);
        }

        private void SendContributionRequestMessage()
        {
            string clientContributionString = Console.ReadLine();
            var clientContribution = new ContributionRequest(new Contribution(clientContributionString));
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(clientContribution, stream);
        }

        private void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            messageReceiver.OnNewMessage += NewMessageReceived;
            messageReceiver.ReceiveMessages(stream);
        }

        private void NewMessageReceived(object sender, MessageEventArgs e)
        {
            IMessage message = e.Message;

            switch (message.Identifier)
            {
                case 1: //Contribution Request
                    Log.Warn("Server shouldn't be sending a ContributionRequest message to a client if following protocol");
                    break;
                case 2: //Contribution Notification
                    var contributionNotification = (ContributionNotification) message;
                    Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
                    Console.WriteLine("The Server sent: " + contributionNotification.Contribution.GetMessage());
                    break;
                case 3: //Login Request
                    Log.Warn("Server shouldn't be sending a LoginRequest message to a client if following protocol");
                    break;
                case 4: //User Notification
                    var userNotification = (UserNotification) message;
                    NotifyClientOfNewUser(userNotification);
                    break;
                default:
                    Log.Warn("Shared classes assembly does not have a definition for message identifier: " + message.Identifier);
                    break;
            }
        }

        private void NotifyClientOfNewUser(UserNotification userNotification)
        {
            connectedUsers.Add(userNotification.User);

            Log.Info("New user logged in successfully, currently connected users: ");

            foreach (var user in connectedUsers)
            {
                Log.Info("User: " + user.UserName);
            }
        }
    }
}