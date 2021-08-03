using System;
using System.ComponentModel.DataAnnotations;

namespace PracticeSockets_Shared.Models
{
    [Serializable]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public int? GroupId { get; set; }
        public Guid SessionId { get; set; }
    }
}
