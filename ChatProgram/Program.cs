using System;
using Server;
using Server.Serialisation;

namespace ChatProgram
{
    internal static class Program
    {
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
            sendBehaviour.Serialise(clientMessage);
        }
    }
}