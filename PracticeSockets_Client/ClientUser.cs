using System;
using System.Threading;
using PracticeSockets_Domain;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Packets;
using PracticeSockets_Shared.Packets.Requests;
using PracticeSockets_Shared.Packets.Responses;

namespace PracticeSockets_Client
{
    public class ClientUser
    {
        private bool _isAuthenticated;
        private string _username;
        private readonly AutoResetEvent _are;
        private readonly ChatClient _chatClient;
        private readonly PacketHandler _packetHandler;

        public ClientUser(string address, int port)
        {
            _isAuthenticated = false;
            _are = new AutoResetEvent(false);

            _chatClient = new ChatClient(address, port);
            _packetHandler = new PacketHandler(_chatClient);

            _packetHandler.LoginResponseReceived += OnLoginResponseReceived;
            _packetHandler.CurrentGroupsReceived += OnGroupsReceived;
            _packetHandler.JoinGroupResponseReceived += OnJoinGroupResponseReceived;
            _packetHandler.LeaveGroupResponseReceived += OnLeaveGroupResponseReceived;
            _packetHandler.ChatMessageReceived += OnChatMessageReceived;
        }

        public void Start()
        {
            _chatClient.ConnectAsync();
            
            Console.Write("Username: ");
            _username = usernameRequest();
            _are.WaitOne();
            
            while (!_isAuthenticated)
            {
                Console.Write("Username is already taken. Please enter another: ");
                _username = usernameRequest();
                _are.WaitOne();
            }

            Console.WriteLine("Successfully logged in");

            groupsRequest();
            _are.WaitOne();
            
            while (true)
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line) || line == "!")
                    break;

                if (!checkAndProcessCommand(line))
                    sendMessage(line);
            }
        }

        public bool Stop()
        {
            return _chatClient.DisconnectAsync();
        }

        public void OnLoginResponseReceived(LoginResponsePacket packet)
        {
            _isAuthenticated = packet.Success;
            _are.Set();
        }

        public void OnGroupsReceived(CurrentGroupsResponsePacket packet)
        {
            if (packet.Groups.Count == 0)
                Console.WriteLine("There are no existing groups. Type !create to create a new group or !help for other options");
            else
            {
                Console.WriteLine("Existing groups");
                foreach (Group group in packet.Groups)
                {
                    Console.WriteLine($"{group.Id}: {group.Users.Count} users");
                }
                Console.WriteLine("Type !join to join a group or !help for other options");
            }
            _are.Set();
        }

        public void OnJoinGroupResponseReceived(JoinGroupResponsePacket packet)
        {
            Console.WriteLine($"{packet.Username} has joined the chat.");
        }

        public void OnLeaveGroupResponseReceived(LeaveGroupResponsePacket packet)
        {
            Console.WriteLine($"{packet.Username} has left the chat.");
        }

        public void OnChatMessageReceived(SendChatMessagePacket packet)
        {
            Console.WriteLine($"{packet.Username}: {packet.Content}");
        }

        private string usernameRequest()
        {
            string username = Console.ReadLine();

            while (String.IsNullOrWhiteSpace(username))
            {
                Console.Write("Username cannot be empty. Please enter another username: ");
                username = Console.ReadLine();
            }

            LoginRequestPacket request = new LoginRequestPacket() { Username = username };
            _chatClient.SendAsync(request);

            return username;
        }

        private void groupsRequest()
        {
            CurrentGroupsRequestPacket request = new CurrentGroupsRequestPacket();
            _chatClient.SendAsync(request);
        }

        private void sendMessage(string message)
        {
            SendChatMessagePacket packet = new SendChatMessagePacket() { Username = _username, Content = message };

            _chatClient.SendAsync(packet);
        }

        private bool checkAndProcessCommand(string line)
        {
            if (!line.StartsWith("!"))
                return false;

            if (line.Equals("!create"))
            {
                createGroup();
            }
            else if (line.StartsWith("!join"))
            {
                int groupId = 0;
                if (line.Length >= 6 && int.TryParse(line[6..], out groupId))
                    joinGroup(groupId);
                else
                {
                    Console.WriteLine("Usage: !join {id}");
                }
            }
            else if (line.StartsWith("!leave"))
            {
                leaveGroup();
            }
            else if (line.StartsWith("!help"))
            {
                Console.WriteLine("Type !join {id} to join an existing group, !create to create a new group, !leave to leave your current group.");
            }
            else
            {
                return false;
            }

            return true;
        }

        private void createGroup()
        {
            CreateGroupRequestPacket request = new CreateGroupRequestPacket() { };

            _chatClient.SendAsync(request);
        }

        private void joinGroup(int id)
        {
            JoinGroupRequestPacket request = new JoinGroupRequestPacket() { GroupId = id };

            _chatClient.SendAsync(request);
        }

        private void leaveGroup()
        {
            LeaveGroupRequestPacket request = new LeaveGroupRequestPacket() { };

            _chatClient.SendAsync(request);
        }
    }
}
