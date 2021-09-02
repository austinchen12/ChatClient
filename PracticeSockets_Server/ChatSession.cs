using System;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using PracticeSockets_Shared.Packets;

namespace PracticeSockets_Server
{
    public class ChatSession : TcpSession
    {
        public new ChatServer Server { get; }

        public ChatSession(ChatServer server)
            : base(server) { Server = server; }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string header = Encoding.UTF8.GetString(buffer[..ChatServer.HeaderSize]).Trim();
            if (header.StartsWith(ChatServer.PacketIdentifier))
            {
                Packet packet = Packet.Deserialize(buffer[ChatServer.HeaderSize..]);
                packet.SessionId = this.Id;
                Server.PacketReceived?.Invoke(packet);
            }
        }

        protected override void OnConnected()
        {
            Server.UserConnected(this.Id);
            Console.WriteLine($"Client connected from session with Id {Id}!");
        }

        protected override void OnDisconnected()
        {
            Server.UserDisconnected(this.Id);
            Console.WriteLine($"Client disconnected from session with Id {Id}!");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }
}
