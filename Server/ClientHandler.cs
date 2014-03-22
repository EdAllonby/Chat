﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using log4net;
using SharedClasses;

namespace Server
{
    public class ClientHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Server));

        private readonly IList<ConnectedClient> connectedClients = new List<ConnectedClient>();

        public ClientHandler()
        {
            Log.Info("New client handler created");
        }

        public void CreateListenerThreadForClient(TcpClient client)
        {
            var newClient = new ConnectedClient(client);
            AddConnectedClient(newClient);
            NetworkStream stream = client.GetStream();
            Log.Info("Stream with client established");

            var messageListenerThread = new Thread(() => ReceiveMessageListener(stream, newClient))
            {
                Name = "MessageListenerThread" + TotalListeners
            };

            messageListenerThread.Start();
        }

        public int TotalListeners { get; private set; }

        public void ReceiveMessageListener(NetworkStream stream, ConnectedClient client)
        {
            bool connection = true;
            TotalListeners++;
            while (connection)
            {
                Message message = Message.Deserialise(stream);

                if (stream.CanRead)
                {
                    Log.Info("Message deserialised. Client sent: " + message.GetMessage());
                    SendMessage(message);
                }
                else
                {
                    connection = false;
                    Log.Warn("Connection is no longer available, stopping server listener thread for this client");
                    RemoveDisconnectedClient(client);
                }
            }
        }

        private void SendMessage(Message message)
        {
            foreach (ConnectedClient client in connectedClients)
            {
                if (client.CurrentStatus == Status.Connected)
                {
                    NetworkStream clientStream = client.Socket.GetStream();
                    message.Serialise(clientStream);
                }
                if (client.CurrentStatus == Status.Disconnected)
                {
                    RemoveDisconnectedClient(client);
                }
            }
        }


        public void AddConnectedClient(ConnectedClient client)
        {
            connectedClients.Add(client);
            Log.Info("Added client to connectedClients list");
        }

        private void RemoveDisconnectedClient(ConnectedClient client)
        {
            connectedClients.Remove(client);
            Log.Info("Client successfully removed from ConnectedClients list");
        }
    }
}