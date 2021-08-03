using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetCoreServer;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_Server
{
    public class ChatSession : TcpSession
    {
        public new ChatServer Server { get; }
        private int _userId;

        public ChatSession(ChatServer server)
            : base(server) { Server = server; }

        protected override void OnConnected()
        {
            Console.WriteLine($"Client connected to session with Id {Id}!");
        }

        protected override void OnDisconnected()
        {
            Server.UserLogoff(_userId);
            Console.WriteLine($"Client disconnected from session with Id {Id}!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (buffer[0] == 33)
            {
                if (login(buffer, offset, size))
                    return;
            }
            
            Message message = Message.Deserialize(buffer);
            
            if (message.Content[0] == '!' && processCommand(message.Content))
                return;

            Server.SendMessage(message);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }

        private bool login(byte[] buffer, long offset, long size)
        {
            string loginContent;
            try
            {
                loginContent = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
                if (loginContent.Contains("!user "))
                {
                    int userId = Server.UserLogin(loginContent.Substring(6), Id);
                    _userId = userId;
                    string response = $"!user {userId}\n";

                    string groupList = "Online Groups:\n";
                    
                    foreach (int groupId in Server.Groups.Keys)
                    {
                        groupList += $"({groupId}) {Server.Groups[groupId].UserIds.Count} members online.\n";
                    }

                    if (Server.Groups.Count == 0)
                        groupList = "No groups currently exist. Type !create to create a group.\n";
                    else
                        groupList += "Type !join {id} to join a group, !leave to leave, and !create to make another group.\n";

                    Server.FindSession(Id).SendAsync(response + groupList);
                    return true;
                }
            }
            catch (DecoderFallbackException)
            { }

            return false;
        }

        private bool processCommand(string message)
        {
            string command = message.Contains(" ") ? message[0..message.IndexOf(" ")] : message;
            string content = message.Contains(" ") ? message[(message.IndexOf(" ") + 1)..] : "";

            switch (command)
            {
                case "!create":
                    Server.CreateGroup(_userId);
                    Server.FindSession(Id).SendAsync("!server Group successfully created. Type !leave to leave.\n");
                    break;
                case "!join":
                    int groupId;
                    if (Int32.TryParse(content, out groupId) && Server.JoinGroup(_userId, groupId))
                        Server.FindSession(Id).SendAsync($"!server Successfully joined group {groupId}. Type !leave to leave.\n");
                    Server.Send(_userId, $"!server User {_userId} has joined.");
                    break;
                case "!leave":
                    Server.Send(_userId, $"!server User {_userId} has left.");
                    if (Server.LeaveGroup(_userId))
                        Server.FindSession(Id).SendAsync("!server You have left the group.\n");
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
