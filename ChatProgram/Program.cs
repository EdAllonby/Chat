using Server;

namespace ChatProgram
{
    class Program
    {
        static void Main()
        {
            var client = new Client("Ed");
            client.NewMessage("Hello");
        }
    }
}