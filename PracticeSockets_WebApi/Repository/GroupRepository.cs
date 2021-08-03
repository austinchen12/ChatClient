using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly PracticeSocketsDbContext _context;

        public GroupRepository(PracticeSocketsDbContext context)
        {
            _context = context;
        }

        public async Task<GroupSQLite> Get(int id)
        {
            return await _context.Group.FindAsync(id);
        }

        public IEnumerable<GroupSQLite> GetAll()
        {
            return _context.Group;
        }

        public async Task Create(GroupSQLite group)
        {
            await _context.Group.AddAsync(group);
            _context.SaveChanges();
        }

        public async Task Update(GroupSQLite group)
        {
            _context.Group.Update(group);
            await _context.SaveChangesAsync();
        }
    }
}
