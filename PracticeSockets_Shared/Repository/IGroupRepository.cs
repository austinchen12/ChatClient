using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_Shared.Repository
{
    public interface IGroupRepository
    {
        public Task<GroupSQLite> Get(int id);
        public IEnumerable<GroupSQLite> GetAll();
        public Task Create(GroupSQLite group);
        public Task Update(GroupSQLite group);
    }
}
