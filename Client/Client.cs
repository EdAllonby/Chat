﻿using System;
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
        private readonly TcpClient connection;

        private readonly LoginRequestSerialiser loginRequestSerialiser = new LoginRequestSerialiser();
        private readonly ContributionNotificationSerialiser contributionNotificationSerialiser = new ContributionNotificationSerialiser();
        private readonly ContributionRequestSerialiser contributionRequestSerialiser = new ContributionRequestSerialiser();

        private readonly IPAddress targetAddress;
        private readonly int targetPort;
        private NetworkStream stream;

        public Client(IPAddress targetAddress, int targetPort)
        {
            this.targetAddress = targetAddress;
            this.targetPort = targetPort;

            Console.Write("Enter name: ");
            string userName = Console.ReadLine();

            try
            {
                connection = ConnectToServer();

                if (connection != null)
                {
                    SendLoginRequest(userName);
                    while (true)
                    {
                        string clientContributionString = Console.ReadLine();
                        var clientContribution = new ContributionRequest {Contribution = new Contribution(clientContributionString)};

                        contributionRequestSerialiser.Serialise(clientContribution, stream);
                    }
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

        private void SendLoginRequest(string userName)
        {
            var loginRequest = new LoginRequest {UserName = userName};
            loginRequestSerialiser.Serialise(loginRequest, stream);
        }

        private TcpClient ConnectToServer()
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
            return client;
        }

        private void ReceiveMessageListener()
        {
            Log.Info("Message listener thread started");
            bool serverConnection = true;
            while (serverConnection)
            {
                ContributionNotification contributionNotification = contributionNotificationSerialiser.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Client sent: " + contributionNotification.Contribution.GetMessage());
                    Console.WriteLine("A client sent: " + contributionNotification.Contribution.GetMessage());
                }
                else
                {
                    serverConnection = false;
                    Log.Warn("Connection is no longer available, stopping client listener thread");
                }
            }
        }
    }
}