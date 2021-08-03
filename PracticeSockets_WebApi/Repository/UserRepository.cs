using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PracticeSocketsDbContext _context;

        public UserRepository(PracticeSocketsDbContext context)
        {
            _context = context;
        }

        public async Task<User> Get(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public User GetByUsername(string username)
        {
            return _context.User
                .Where(u => u.Username.Equals(username))
                .FirstOrDefault();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public async Task Create(User user)
        {
            await _context.User.AddAsync(user);
        }

        public async Task Update(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
