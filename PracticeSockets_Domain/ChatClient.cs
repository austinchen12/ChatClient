  using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using PracticeSockets_Shared.Packets;
using TcpClient = NetCoreServer.TcpClient;

namespace PracticeSockets_Domain
{
    public class ChatClient : TcpClient
    {
        public event Action Authenticated;
        public event Action<Packet> PacketReceived;
        private const int HeaderSize = 50;
        private const string PacketIdentifier = "socketpractice";

        public ChatClient(string address, int port)
            : base(address, port) { }

        public bool SendAsync(Packet packet)
        {
            byte[] header = createHeader(packet);
            byte[] content = packet.Serialize();

            byte[] data = createSocketData(header, content);

            return SendAsync(data);
        }

        private byte[] createHeader(Packet packet)
        {
            return Encoding.UTF8.GetBytes($"{PacketIdentifier}-{packet.GetType().Name}");
        }

        private byte[] createSocketData(byte[] header, byte[] content)
        {
            byte[] data = new byte[HeaderSize + content.Length];
            Array.Copy(header, data, header.Length);
            Array.Copy(content, 0, data, HeaderSize, content.Length);

            return data;
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string header = Encoding.UTF8.GetString(buffer[..HeaderSize]).Trim();
            if (header.StartsWith(PacketIdentifier))
            {
                Packet packet = Packet.Deserialize(buffer[HeaderSize..]);

                PacketReceived?.Invoke(packet);
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }
    }
}
