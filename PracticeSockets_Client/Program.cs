using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 7777;

            User user = new User() { Id = 1, Username = "Austin" };

            var client = new ChatClient(address, port, user);
            client.ConnectAsync();

            while (true)
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line) || line == "!")
                    break;

                client.SendMessageAsync(line);
            }

            client.DisconnectAndStop();
        }
    }
}
