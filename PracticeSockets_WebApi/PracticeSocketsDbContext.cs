using System;
using Microsoft.EntityFrameworkCore;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_WebApi
{
    public class PracticeSocketsDbContext : DbContext
    {
        public PracticeSocketsDbContext(DbContextOptions<PracticeSocketsDbContext> options)
            : base(options)
        { }

        public DbSet<Message> Message { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<GroupSQLite> Group { get; set; }
    }
}
