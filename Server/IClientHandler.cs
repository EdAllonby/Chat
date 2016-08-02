using System;
using System.Net.Sockets;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    public interface IClientHandler
    {
        /// <summary>
        /// Fires when a message has been sent from the client.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Logs in a requested Client to the Server.
        /// </summary>
        /// <param name="tcpClient">The client's connection.</param>
        /// <param name="serviceRegistry">Holds services to initialise client</param>
        /// <returns>A login response <see cref="IMessage" /> with the details of the login attempt.</returns>
        LoginResponse InitialiseClient(TcpClient tcpClient, IServiceRegistry serviceRegistry);

        /// <summary>
        /// Send an <see cref="IMessage" /> to the client.
        /// </summary>
        /// <param name="message">The <see cref="IMessage" /> to send to the client.</param>
        void SendMessage(IMessage message);
    }
}