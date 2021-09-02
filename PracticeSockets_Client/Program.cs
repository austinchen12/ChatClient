using System;

namespace PracticeSockets_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ClientUser("127.0.0.1", 7777);
            client.Start();
        }
    }
}
