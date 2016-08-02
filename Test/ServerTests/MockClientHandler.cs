using System;
using System.Net.Sockets;
using Server;
using SharedClasses;
using SharedClasses.Message;

namespace ServerTests
{
    public sealed class MockClientHandler : IClientHandler
    {
        public event EventHandler<MessageEventArgs> MessageReceived;

        public LoginResponse InitialiseClient(TcpClient tcpClient, IServiceRegistry serviceRegistry)
        {
            return null;
        }

        public void SendMessage(IMessage message)
        {
            OnMessageSent(message);
        }

        public event EventHandler<MessageEventArgs> MessageSent;

        private void OnMessageSent(IMessage message)
        {
            EventHandler<MessageEventArgs> messageSentCopy = MessageSent;
            if (messageSentCopy != null)
            {
                messageSentCopy(this, new MessageEventArgs(message));
            }
        }
    }
}