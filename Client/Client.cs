using System;
using System.Net.Sockets;
using SharedClasses;

namespace Client
{
    internal class Client
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Client));

        public Client()
        {
            Log.Info("Client looking for server");

            var client = new TcpClient("localhost", 5004);
            Log.Info("Client found server, connection created");

            //get the network stream
            NetworkStream stream = client.GetStream();
            Log.Info("Created stream with Server");

            while (true)
            {
                try
                {
                    Console.WriteLine("Say: ");
                    string test = Console.ReadLine();
                    var message = new Message(test);

                    Log.Info("Attempt to serialise message and send to server");
                    message.Serialise(stream);
                }
                catch (Exception e)
                {
                    Log.Error(e);

                    //close the client and stream
                    stream.Close();
                    Log.Info("Stream closed");
                    client.Close();
                    Log.Info("Client connection closed");
                }
            }
        }
    }
}