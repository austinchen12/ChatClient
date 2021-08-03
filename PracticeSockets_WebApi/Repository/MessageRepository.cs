using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly PracticeSocketsDbContext _context;

        public MessageRepository(PracticeSocketsDbContext context)
        {
            _context = context;
        }

        public async Task<Message> Get(int id)
        {
            return await _context.Message.FindAsync(id);
        }

        public IEnumerable<Message> GetAll()
        {
            return _context.Message;
        }

        public async Task Create(Message message)
        {
            await _context.Message.AddAsync(message);
            _context.SaveChanges();
        }

        public async Task Update(Message message)
        {
            _context.Message.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}
