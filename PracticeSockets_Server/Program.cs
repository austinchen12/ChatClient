using System;
using System.Net;

namespace PracticeSockets_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 7777;

            var server = new ChatServer(IPAddress.Parse("127.0.0.1"), port);
            server.Start();

            while (true)
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line) || line == "!")
                    break;

                line = "(admin) " + line;

                server.Multicast(line);
            }

            server.Stop();
        }
    }
}
