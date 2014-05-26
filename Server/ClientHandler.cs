using System.Collections.Generic;
using System.Net.Sockets;
using SharedClasses;
using SharedClasses.Message;

namespace Server
{
    internal class ClientHandler
    {
        private readonly IDictionary<int, ConnectionHandler> clientConnectionHandlersIndexedByUserId = new Dictionary<int, ConnectionHandler>();

        public void AddNewConnectionHandler(int userId, TcpClient tcpClient)
        {
            clientConnectionHandlersIndexedByUserId[userId] = new ConnectionHandler(userId, tcpClient);
        }

        public ConnectionHandler GetConnectionHandler(int userId)
        {
            return clientConnectionHandlersIndexedByUserId[userId];
        }

        public void RemoveConnectionHander(int userId)
        {
            clientConnectionHandlersIndexedByUserId.Remove(userId);
        }

        public void NotifyAllClients(IMessage message)
        {
            foreach (ConnectionHandler handler in clientConnectionHandlersIndexedByUserId.Values)
            {
                handler.SendMessage(message);
            }
        }
    }
}