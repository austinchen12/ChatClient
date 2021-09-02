using System;
using System.Collections.Generic;

namespace PracticeSockets_Shared.Models
{
    [Serializable]
    public class Group
    {
        public int Id { get; set; }
        public List<User> Users { get; set; }
    }
}
