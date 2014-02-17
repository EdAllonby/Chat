using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Serialisation
{
    public class BinaryFormat : ITcpSendBehaviour
    {
        public void Serialise(Client clientMessage)
        {
            TcpClient client = null;

            try
            {
                client = new TcpClient("localhost", ServerData.portNumber);

                var binaryFormatter = new BinaryFormatter();

                Stream networkStream = client.GetStream();

                if (networkStream.CanWrite)
                {
                    binaryFormatter.Serialize(networkStream, clientMessage);
                }
                networkStream.Close();
            }
            catch (SocketException)
            {
                Console.WriteLine("Check that the server is running and you've set the right port and IPAddress");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        public Client Deserialise(Stream networkStream)
        {
            try
            {
                while (true)
                {
                    var binaryFormatter = new BinaryFormatter();
                    Client client = null;

                    if (networkStream.CanRead)
                    {
                        client = (Client) binaryFormatter.Deserialize(networkStream);
                    }
                    return client;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}