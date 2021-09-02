using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Packets;
using PracticeSockets_Shared.Packets.Requests;
using PracticeSockets_Shared.Packets.Responses;

namespace PracticeSockets_Server
{
    public class ChatServer : TcpServer
    {
        public Dictionary<Guid, User> OnlineUsers { get; }
        public Dictionary<int, Group> Groups { get; }
        private readonly PacketHandler _packetHandler;
        public const int HeaderSize = 50;
        public const string PacketIdentifier = "socketpractice";

        public Action<Packet> PacketReceived;

        public ChatServer(IPAddress address, int port)
            : base(address, port)
        {
            _packetHandler = new PacketHandler(this);
            _packetHandler.LoginRequestReceived += OnLoginRequestReceived;
            _packetHandler.CurrentGroupsRequestReceived += OnCurrentGroupsRequestReceived;
            _packetHandler.CreateGroupRequestReceived += OnCreateGroupRequestReceived;
            _packetHandler.JoinGroupRequestReceived += OnJoinGroupRequestReceived;
            _packetHandler.LeaveGroupRequestReceived += OnLeaveGroupRequestReceived;
            _packetHandler.MessageReceived += OnMessageReceived;

            OnlineUsers = new Dictionary<Guid, User>();
            Groups = new Dictionary<int, Group>();
        }

        protected override TcpSession CreateSession() { return new ChatSession(this); }

        public void UserConnected(Guid sessionId)
        {
            User user = new User() { Username = null, SessionId = sessionId, GroupId = null };
            OnlineUsers.Add(sessionId, user);
        }

        public void UserDisconnected(Guid sessionId)
        {
            User user = OnlineUsers[sessionId];
            if (user.GroupId.HasValue)
                Groups[user.GroupId.Value].Users.Remove(user);
            OnlineUsers.Remove(sessionId);
        }

        public void OnLoginRequestReceived(LoginRequestPacket packet)
        {
            LoginResponsePacket response = new LoginResponsePacket()
            {
                Success = !OnlineUsers.Values.Select(u => u.Username).Contains(packet.Username)
            };

            if (response.Success)
                OnlineUsers[packet.SessionId].Username = packet.Username;
            
            sendAsSocketData(response, packet.SessionId);
        }

        public void OnCurrentGroupsRequestReceived(CurrentGroupsRequestPacket packet)
        {
            sendGroupsData(packet.SessionId);
        }

        public void OnCreateGroupRequestReceived(CreateGroupRequestPacket packet)
        {
            User user = OnlineUsers[packet.SessionId];
            if (user.GroupId.HasValue)
                return;

            int groupId = Groups.Count;

            Groups[groupId] = new Group() { Id = groupId, Users = new List<User>() { user} };
            user.GroupId = groupId;
        }

        public void OnJoinGroupRequestReceived(JoinGroupRequestPacket packet)
        {
            if (!Groups.ContainsKey(packet.GroupId))
                return;

            User user = OnlineUsers[packet.SessionId];

            if (user.GroupId == packet.GroupId)
                return;

            user.GroupId = packet.GroupId;

            Groups[packet.GroupId].Users.Add(user);

            JoinGroupResponsePacket response = new JoinGroupResponsePacket() { Username = user.Username };
            List<Guid> ids = Groups[packet.GroupId].Users.Select(u => u.SessionId).ToList();
            ids.Remove(packet.SessionId);

            sendAsSocketData(response, ids);
        }

        public void OnLeaveGroupRequestReceived(LeaveGroupRequestPacket packet)
        {
            if (!OnlineUsers[packet.SessionId].GroupId.HasValue)
                return;

            User user = OnlineUsers[packet.SessionId];
            int groupId = user.GroupId.Value;
            Groups[groupId].Users.Remove(user);
            OnlineUsers[user.SessionId].GroupId = null;

            if (Groups[groupId].Users.Count != 0)
            {
                LeaveGroupResponsePacket response = new LeaveGroupResponsePacket() { Username = user.Username };
                List<Guid> ids = Groups[groupId].Users.Select(u => u.SessionId).ToList();
                ids.Remove(packet.SessionId);

                sendAsSocketData(response, ids);
            }
            else
            {
                Groups.Remove(groupId);
            }
        }

        public void OnMessageReceived(SendChatMessagePacket packet)
        {
            User user = OnlineUsers[packet.SessionId];
            if (!user.GroupId.HasValue)
            {
                return;
            }

            Group group = Groups[user.GroupId.Value];

            List<Guid> ids = group.Users.Select(u => u.SessionId).ToList();
            ids.Remove(user.SessionId);

            sendAsSocketData(packet, ids);
        }

        private byte[] createHeader(Packet packet)
        {
            return Encoding.UTF8.GetBytes($"{PacketIdentifier}-{packet.GetType().Name}");
        }

        private bool sendAsSocketData(Packet packet, Guid sessionId)
        {
            byte[] header = createHeader(packet);
            byte[] content = packet.Serialize();

            byte[] data = new byte[HeaderSize + content.Length];
            Array.Copy(header, data, header.Length);
            Array.Copy(content, 0, data, HeaderSize, content.Length);
            
            return FindSession(sessionId).SendAsync(data);
        }

        private void sendAsSocketData(Packet packet, IEnumerable<Guid> sessionIds)
        {
            foreach (Guid id in sessionIds)
            {
                sendAsSocketData(packet, id);
            }
        }

        private bool sendGroupsData(Guid sessionId)
        {
            CurrentGroupsResponsePacket groups = new CurrentGroupsResponsePacket()
            {
                Groups = Groups.Values.ToList()
            };
            
            return sendAsSocketData(groups, sessionId);
        }

        protected override void OnStarted()
        {
            Console.WriteLine("Server started.");
        }

        protected override void OnStopped()
        {
            Console.WriteLine("Server stopped.");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat server caught an error with code {error}");
        }
    }
}
