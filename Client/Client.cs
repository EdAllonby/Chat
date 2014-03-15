using System;
using System.IO;
using System.Net.Sockets;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Client));

        private static TcpClient tcpClient;

        static Client()
        {
            CreateTCPConnection();
        }

        private static void CreateTCPConnection()
        {
            try
            {
                tcpClient = new TcpClient("localhost", 5004);
                while (true)
                {
                    Console.Write("Message: ");

                    string message = Console.ReadLine();
                    var clientMessage = new Message(message);

                    SendMessage(clientMessage);
                }
            }
            catch (SocketException socketException)
            {
                Log.Error("No connection to server", socketException);
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    Log.Info("TCP Connection successfully closed");
                }
            }

        }

        private static void SendMessage(Message clientMessage)
        {
            Stream networkStream = tcpClient.GetStream();
            clientMessage.Serialise(networkStream);
        }
    }
}