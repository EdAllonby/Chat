using System;
using System.IO;
using System.Net.Sockets;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private static TcpClient tcpClient;

        public Client()
        {
            while (true)
            {
                Console.Write("Message: ");
                string message = Console.ReadLine();
                var clientMessage = new Message(message);

                SendMessage(clientMessage);
            }
        }

        private static void SendMessage(Message clientMessage)
        {
            try
            {
                tcpClient = new TcpClient("localhost", 5004);
                Stream networkStream = tcpClient.GetStream();

                clientMessage.Serialise(networkStream);

                tcpClient.Close();
            }
            catch (SocketException socketException)
            {
            }
            catch (Exception exception)
            {
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }
        }
    }
}