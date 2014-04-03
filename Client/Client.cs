using System;
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

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;
        private readonly string userName;
        private TcpClient connection;
        private NetworkStream stream;

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
                    SendContributionMessage();
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

        private void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            bool serverConnection = true;
            while (serverConnection)
            {
                if (stream.CanRead)
                {
                    ReceiveMessage();
                }
                else
                {
                    serverConnection = false;
                    Log.Warn("Connection is no longer available, stopping client listener thread");
                }
            }
        }

        private void ReceiveMessage()
        {
            int messageIdentifier = MessageIdentifierSerialiser.DeserialiseMessageIdentifier(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            IMessage message = serialiser.Deserialise(stream);

            switch (messageIdentifier)
            {
                case 1:
                    ContributionRequest contributionRequest = (ContributionRequest) message;
                    Log.Info("Server sent: " + contributionRequest.Contribution.GetMessage());
                    Console.WriteLine("The Server sent: " + contributionRequest.Contribution.GetMessage());
                    break;
                case 2:
                    ContributionNotification contributionNotification = (ContributionNotification) message;
                    Log.Info("Server sent: " + contributionNotification.Contribution.GetMessage());
                    Console.WriteLine("The Server sent: " + contributionNotification.Contribution.GetMessage());
                    break;
                case 3:
                    LoginRequest loginRequest = (LoginRequest) message;
                    Log.Info("Server sent: " + loginRequest.UserName);
                    Console.WriteLine("The Server sent: " + loginRequest.UserName);
                    break;
            }
        }

        private void SendContributionMessage()
        {
            string clientContributionString = Console.ReadLine();
            var clientContribution = new ContributionRequest(new Contribution(clientContributionString));
            SendMessage(clientContribution);
        }

        private void SendMessage(IMessage message)
        {
            ISerialiser serialiser = serialiserFactory.GetSerialiser<ContributionRequest>();
            serialiser.Serialise(message, stream);
        }
    }
}