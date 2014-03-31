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
                    SendNewContributionRequest();
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
            ISerialiser loginRequestSerialiser = new LoginRequestSerialiser();
            var loginRequest = new LoginRequest {UserName = userName};
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
                    ReceiveContributionNotification();
                }
                else
                {
                    serverConnection = false;
                    Log.Warn("Connection is no longer available, stopping client listener thread");
                }
            }
        }

        private void ReceiveContributionNotification()
        {
            int messageIdentity = MessageType.Deserialise(stream);

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentity);

            IMessage contributionNotification = serialiser.Deserialise(stream);

            Log.Debug("Client sent Contribution notification message");
            Log.Info("Client sent: " + contributionNotification.GetMessage());
            Console.WriteLine("A client sent: " + contributionNotification.GetMessage());
        }

        private void SendNewContributionRequest()
        {
            ISerialiser contributionRequestSerialiser = new ContributionRequestSerialiser();
            string clientContributionString = Console.ReadLine();
            var clientContribution = new ContributionRequest(new Contribution(clientContributionString));
            contributionRequestSerialiser.Serialise(clientContribution, stream);
        }
    }
}