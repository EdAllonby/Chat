using System;
using System.IO;
using System.Net.Sockets;
using Server;
using Server.Serialisation;

namespace ChatProgram
{
    internal static class Program
    {
        private static TcpClient client = null;

        private static string name;
        private static string message;

        private static ITcpSendBehaviour sendBehaviour;

        private static void Main()
        {
            sendBehaviour = new BinaryFormat();

            Console.WriteLine("What is your name");
            name = Console.ReadLine();

            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("You have to specify a name");
                name = Console.ReadLine();
            }

            var client = new Client(name);

            SendMessage(client);

            string file = @"C:\test.txt";

            do
            {
                Console.Write("Chat: ");

                message = Console.ReadLine();

                var clientMessage = new Client(name, message);
                SendMessage(clientMessage);
            } while (true);
        }

        private static void SendMessage(Client clientMessage)
        {
            try
            {
                client = new TcpClient("localhost", ServerData.portNumber);
                Stream networkStream = client.GetStream();

                sendBehaviour.Serialise(networkStream, clientMessage);

                client.Close();
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Check that the server is running and you've set the right port and IPAddress");
                Console.WriteLine();
                Console.WriteLine("--------------------------------");
                Console.WriteLine();
                Console.WriteLine(socketException);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();    
                }
            }
        }
    }
}