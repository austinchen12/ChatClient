using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetCoreServer;
using PracticeSockets_Shared.Models;
using HttpClient = System.Net.Http.HttpClient;
using Newtonsoft.Json;

namespace PracticeSockets_Server
{
    public class ChatServer : TcpServer
    {
        private readonly HttpClient _client;
        public Dictionary<int, User> OnlineUsers { get; }
        public Dictionary<int, Group> Groups { get; }

        public ChatServer(IPAddress address, int port)
            : base(address, port)
        {
            _client = new HttpClient();
            OnlineUsers = new Dictionary<int, User>();
            Groups = new Dictionary<int, Group>();
        }

        protected override TcpSession CreateSession() { return new ChatSession(this); }

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

        public int UserLogin(string username, Guid sessionId)
        {
            User user = new User() { Id = OnlineUsers.Count, Username = username, SessionId = sessionId, GroupId = null };
            OnlineUsers.Add(user.Id, user);

            return user.Id;
        }

        public void UserLogoff(int userId)
        {
            LeaveGroup(userId);
            OnlineUsers.Remove(userId);
        }

        public bool Send(int userId, string message)
        {
            User sender = OnlineUsers[userId];
            if (!sender.GroupId.HasValue)
                return false;

            Group group = Groups[sender.GroupId.Value];

            foreach (int id in group.UserIds)
            {
                if (id == sender.Id)
                    continue;

                if (!FindSession(OnlineUsers[id].SessionId).SendAsync(message))
                    return false;
            }

            return true;
        }

        public bool SendMessage(Message message)
        {
            User sender = OnlineUsers[message.UserId];
            if (!sender.GroupId.HasValue)
                return false;

            Group group = Groups[sender.GroupId.Value];

            foreach (int userId in group.UserIds)
            {
                if (userId == sender.Id)
                    continue;

                if (!FindSession(OnlineUsers[userId].SessionId).SendAsync(message.Serialize()))
                    return false;
            }

            return true;
        }

        public void CreateGroup(int userId)
        {
            int groupId = Groups.Count;
            Groups[groupId] = new Group() { Id = groupId, UserIds = new List<int>() { userId } };
            OnlineUsers[userId].GroupId = groupId;
        }

        public bool JoinGroup(int userId, int groupId)
        {
            if (!Groups.ContainsKey(groupId))
                return false;

            Groups[groupId].UserIds.Add(userId);
            OnlineUsers[userId].GroupId = groupId;

            return true;
        }

        public bool LeaveGroup(int userId)
        {
            if (!OnlineUsers[userId].GroupId.HasValue)
                return false;

            int groupId = OnlineUsers[userId].GroupId.Value;
            Groups[groupId].UserIds.Remove(userId);
            OnlineUsers[userId].GroupId = null;

            if (Groups[groupId].UserIds.Count == 0)
                Groups.Remove(groupId);

            return true;
        }
    }
}
