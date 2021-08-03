using System;
using System.Collections.Generic;

namespace PracticeSockets_Shared.Models
{
    public class Group
    {
        public int Id { get; set; }
        public List<int> UserIds { get; set; }
    }
}
