using System;

namespace PracticeSockets_Shared.Models
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public int? GroupId { get; set; }
        public Guid SessionId { get; set; }
    }
}
