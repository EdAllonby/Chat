using System;
using System.IO;
using System.Net.Sockets;
using Server;
using Server.Serialisation;

namespace ChatProgram
{
    internal static class Program
    {
        private static TcpClient client;

        private static string clientName;
        private static string clientMessage;

        private static ITcpSendBehaviour sendBehaviour;

        private static void Main()
        {
            Client joinedClient = InitialiseClientProgram();

            SendMessage(joinedClient);

            do
            {
                Console.Write("Chat: ");

                Program.clientMessage = Console.ReadLine();

                var clientMessage = new Client(clientName, Program.clientMessage);
                SendMessage(clientMessage);
            } while (true);
        }

        private static Client InitialiseClientProgram()
        {
            SetBehaviour(new BinaryFormat());

            SetClientName();

            var joinedClient = new Client(clientName);

            return joinedClient;
        }

        private static void SetBehaviour(ITcpSendBehaviour wantedSendBehaviour)
        {
            sendBehaviour = wantedSendBehaviour;
        }

        private static void SetClientName()
        {
            Console.WriteLine("What is your name");
            clientName = Console.ReadLine();

            while (string.IsNullOrEmpty(clientName))
            {
                Console.WriteLine("You have to specify a name");
                clientName = Console.ReadLine();
            }
        }

        private static void SendMessage(Client clientMessage)
        {
            try
            {
                client = new TcpClient("localhost", ServerData.PortNumber);
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