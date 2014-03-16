using System;
using System.Net.Sockets;
using System.Threading;
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

            NetworkStream stream = client.GetStream();
            Log.Info("Created stream with Server");

            var messageListenerThread = new Thread(() => ReceiveMessageListener(stream, client))
            {
                Name = "MessageListenerThread"
            };
            messageListenerThread.Start();
            while (true)
            {
                try
                {
                    string test = Console.ReadLine();
                    var message = new Message(test);

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

        private static void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            Log.Info("Message listener thread started");
            bool connection = true;
            while (connection)
            {
                try
                {
                    var message = Message.Deserialise(stream);
                    Log.Info("This client sent: " + message.Text + " at: " + message.MessageTimeStamp);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    stream.Close();
                    Log.Info("Client stream closed");
                    client.Close();
                    Log.Info("Client connection closed");
                    connection = false;
                }
            }
        }
    }
}