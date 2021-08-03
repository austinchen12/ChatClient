using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_Shared.Repository
{
    public interface IUserRepository
    {
        public Task<User> Get(int id);
        public User GetByUsername(string username);
        public IEnumerable<User> GetAll();
        public Task Create(User user);
        public Task Update(User user);
    }
}
