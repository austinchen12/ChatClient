using System;
using PracticeSockets_Shared.Packets;
using PracticeSockets_Shared.Packets.Requests;

namespace PracticeSockets_Server
{
    public class PacketHandler
    {
        public event Action<LoginRequestPacket> LoginRequestReceived;
        public event Action<CurrentGroupsRequestPacket> CurrentGroupsRequestReceived;
        public event Action<CreateGroupRequestPacket> CreateGroupRequestReceived;
        public event Action<JoinGroupRequestPacket> JoinGroupRequestReceived;
        public event Action<LeaveGroupRequestPacket> LeaveGroupRequestReceived;
        public event Action<SendChatMessagePacket> MessageReceived;

        public PacketHandler(ChatServer server)
        {
            server.PacketReceived += OnLoginRequestReceived;
            server.PacketReceived += OnCurrentGroupsRequestReceived;
            server.PacketReceived += OnCreateGroupRequestReceived;
            server.PacketReceived += OnJoinGroupRequestReceived;
            server.PacketReceived += OnLeaveGroupRequestReceived;
            server.PacketReceived += OnMessageReceived;
        }

        public void OnLoginRequestReceived(Packet packet)
        {
            if (packet is LoginRequestPacket loginRequestPacket)
            {
                LoginRequestReceived?.Invoke(loginRequestPacket);
            }
        }

        public void OnCurrentGroupsRequestReceived(Packet packet)
        {
            if (packet is CurrentGroupsRequestPacket currentGroupsRequestPacket)
            {
                CurrentGroupsRequestReceived?.Invoke(currentGroupsRequestPacket);
            }
        }

        public void OnCreateGroupRequestReceived(Packet packet)
        {
            if (packet is CreateGroupRequestPacket createGroupRequestPacket)
            {
                CreateGroupRequestReceived?.Invoke(createGroupRequestPacket);
            }
        }

        public void OnJoinGroupRequestReceived(Packet packet)
        {
            if (packet is JoinGroupRequestPacket joinGroupRequestPacket)
            {
                JoinGroupRequestReceived?.Invoke(joinGroupRequestPacket);
            }
        }

        public void OnLeaveGroupRequestReceived(Packet packet)
        {
            if (packet is LeaveGroupRequestPacket leaveGroupRequestPacket)
            {
                LeaveGroupRequestReceived?.Invoke(leaveGroupRequestPacket);
            }
        }

        public void OnMessageReceived(Packet packet)
        {
            if (packet is SendChatMessagePacket messagePacket)
            {
                MessageReceived?.Invoke(messagePacket);
            }
        }
    }
}
