using System;
using PracticeSockets_Shared.Packets;
using PracticeSockets_Shared.Packets.Responses;

namespace PracticeSockets_Domain
{
    public class PacketHandler
    {
        public event Action<LoginResponsePacket> LoginResponseReceived;
        public event Action<CurrentGroupsResponsePacket> CurrentGroupsReceived;
        public event Action<JoinGroupResponsePacket> JoinGroupResponseReceived;
        public event Action<LeaveGroupResponsePacket> LeaveGroupResponseReceived;
        public event Action<SendChatMessagePacket> ChatMessageReceived;

        public PacketHandler(ChatClient client)
        {
            client.PacketReceived += OnLoginResponseReceived;
            client.PacketReceived += OnGroupsReceived;
            client.PacketReceived += OnChatMessageReceieved;
            client.PacketReceived += OnJoinGroupResponseReceived;
            client.PacketReceived += OnLeaveGroupResponseReceived;
        }

        public void OnLoginResponseReceived(Packet packet)
        {
            if (packet is LoginResponsePacket loginResponsePacket)
            {
                LoginResponseReceived?.Invoke(loginResponsePacket);
            }
        }

        public void OnGroupsReceived(Packet packet)
        {
            if (packet is CurrentGroupsResponsePacket currentGroupsPacket)
            {
                CurrentGroupsReceived?.Invoke(currentGroupsPacket);
            }
        }

        public void OnJoinGroupResponseReceived(Packet packet)
        {
            if (packet is JoinGroupResponsePacket joinGroupResponsePacket)
            {
                JoinGroupResponseReceived?.Invoke(joinGroupResponsePacket);
            }
        }

        public void OnLeaveGroupResponseReceived(Packet packet)
        {
            if (packet is LeaveGroupResponsePacket leaveGroupResponsePacket)
            {
                LeaveGroupResponseReceived?.Invoke(leaveGroupResponsePacket);
            }
        }

        public void OnChatMessageReceieved(Packet packet)
        {
            if (packet is SendChatMessagePacket sendChatMessagePacket)
            {
                ChatMessageReceived?.Invoke(sendChatMessagePacket);
            }
        }
    }
}
