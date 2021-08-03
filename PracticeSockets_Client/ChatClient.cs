using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PracticeSockets_Shared.Models;
using TcpClient = NetCoreServer.TcpClient;

namespace PracticeSockets_Client
{
    class ChatClient : TcpClient
    {
        private readonly HttpClient _client;
        private readonly User _user;

        public ChatClient(string address, int port, User user) : base(address, port)
        {
            _user = user;
            _client = new HttpClient();
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            loginAsync($"{_user.Username}");
            Console.WriteLine("Connected to server.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected from server.");

            Thread.Sleep(1000);

            if (!_stop)
                ConnectAsync();
        }

        public bool SendMessageAsync(string content)
        {
            Message message = new Message()
            {
                UserId = _user.Id,
                Content = content,
                Timestamp = DateTime.Now
            };

            byte[] messageData = message.Serialize();

            bool result = SendAsync(messageData);

            //StringContent stringContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await _client.PostAsync("https://localhost:4462/message", stringContent);

            return result;
        }

        private bool loginAsync(string username)
        {
            return SendAsync($"!user {username}");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (!(buffer[0] == 33))
            {
                Message message = Message.Deserialize(buffer);
                Console.WriteLine($"[{message.Timestamp}] {message.UserId}: {message.Content}");
            }

            string loginContent;
            try
            {
                loginContent = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

                if (loginContent.Contains("!user "))
                {
                    _user.Id = Int32.Parse(loginContent[6..loginContent.IndexOf('\n')]);
                    Console.WriteLine(loginContent[(loginContent.IndexOf('\n') + 1)..]);
                    return;
                }
                else if (loginContent.Contains("!server "))
                {
                    Console.WriteLine(loginContent[8..]);
                }
            }
            catch (DecoderFallbackException)
            { }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}
