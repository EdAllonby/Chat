using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharedClasses;

namespace Server
{
    internal class Server
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof (Server));

        public Server()
        {
            //listen for connections from tcp network clients
            var server1 = new TcpListener(IPAddress.Loopback, 5004);

            //start listening
            server1.Start();
            Log.Info("Server started listening for clients to connect");

            //accepts a pending connection request
            TcpClient client = server1.AcceptTcpClient();
            Log.Info("New client connected");

            //get the network stream
            NetworkStream stream = client.GetStream();
            Log.Info("Stream with client established");

            var messsageListenerThread = new Thread(() => ReceiveMessageListener(stream, client)) {Name = "MessageListenerThread"};
            messsageListenerThread.Start();
        }

        private void ReceiveMessageListener(NetworkStream stream, TcpClient client)
        {
            bool connection = true;
            while (connection)
            {
                try
                {
                    // you have to cast the deserialized object
                    var addressInfo = Message.Deserialise(stream);

                    Log.Info("Message deserialised. Client sent: " + addressInfo.Text + " at: " +
                             addressInfo.MessageTimeStamp);
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