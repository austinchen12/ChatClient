using System;
using System.ComponentModel.DataAnnotations;

namespace PracticeSockets_Shared.Models
{
    public class GroupSQLite
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserIds { get; set; }
    }
}
