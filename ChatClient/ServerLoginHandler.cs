﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatClient.ViewModels.LoginWindowViewModel;
using log4net;
using SharedClasses;
using SharedClasses.Message;
using SharedClasses.Serialiser;

namespace ChatClient
{
    /// <summary>
    /// Creates a connection to the Server and gets the entity snapshots the client needs.
    /// </summary>
    internal sealed class ServerLoginHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServerLoginHandler));

        private readonly SerialiserFactory serialiserFactory = new SerialiserFactory();

        private int clientUserId;
        private TcpClient serverConnection;

        public ConnectionHandler CreateServerConnectionHandler()
        {
            return new ConnectionHandler(clientUserId, serverConnection);
        }

        public InitialisedData ConnectToServer(string username, IPAddress targetIPAddress, int targetPort)
        {
            CreateConnection(targetIPAddress, targetPort);

            IMessage userRequest = new LoginRequest(username);
            SendConnectionMessage(userRequest, serverConnection);
            LoginResponse loginResponse = GetLoginResponse(serverConnection);

            switch (loginResponse.LoginResult)
            {
                case LoginResult.Success:
                    clientUserId = loginResponse.User.UserId;
                    return GetSnapshots(clientUserId);
                case LoginResult.AlreadyConnected:
                    throw new UserAlreadyConnectedException();
                default:
                    // This case will never happen
                    return new InitialisedData(0, null, null, null);
            }
        }

        private void CreateConnection(IPAddress targetAddress, int targetPort)
        {
            const int TimeoutSeconds = 5;

            Log.Info("Client looking for server with address: " + targetAddress + ":" + targetPort);

            var connection = new TcpClient();

            connection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            IAsyncResult asyncResult = connection.BeginConnect(targetAddress.ToString(), targetPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), false))
                {
                    connection.Close();
                    throw new TimeoutException();
                }

                connection.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }

            Log.Info("Client found server, connection created");
            serverConnection = connection;
        }

        private void SendConnectionMessage(IMessage message, TcpClient tcpClient)
        {
            ISerialiser messageSerialiser = serialiserFactory.GetSerialiser(message.Identifier);
            messageSerialiser.Serialise(message, tcpClient.GetStream());
        }

        private LoginResponse GetLoginResponse(TcpClient tcpClient)
        {
            return (LoginResponse) GetConnectionIMessage(tcpClient);
        }

        private InitialisedData GetSnapshots(int userId)
        {
            SendConnectionMessage(new UserSnapshotRequest(), serverConnection);

            UserSnapshot userSnapshot = GetUserSnapshot(serverConnection);

            SendConnectionMessage(new ConversationSnapshotRequest(), serverConnection);

            ConversationSnapshot conversationSnapshot = GetConversationSnapshot(serverConnection);

            SendConnectionMessage(new ParticipationSnapshotRequest(), serverConnection);

            ParticipationSnapshot participationSnapshot = GetParticipationSnapshot(serverConnection);

            return new InitialisedData(userId, userSnapshot, conversationSnapshot, participationSnapshot);
        }

        private UserSnapshot GetUserSnapshot(TcpClient tcpClient)
        {
            return (UserSnapshot) GetConnectionIMessage(tcpClient);
        }

        private ConversationSnapshot GetConversationSnapshot(TcpClient tcpClient)
        {
            return (ConversationSnapshot) GetConnectionIMessage(tcpClient);
        }

        private ParticipationSnapshot GetParticipationSnapshot(TcpClient tcpClient)
        {
            return (ParticipationSnapshot) GetConnectionIMessage(tcpClient);
        }

        private IMessage GetConnectionIMessage(TcpClient tcpClient)
        {
            var messageIdentifierSerialiser = new MessageIdentifierSerialiser();

            MessageNumber messageIdentifier = messageIdentifierSerialiser.DeserialiseMessageIdentifier(tcpClient.GetStream());

            ISerialiser serialiser = serialiserFactory.GetSerialiser(messageIdentifier);

            return serialiser.Deserialise(tcpClient.GetStream());
        }
    }
}