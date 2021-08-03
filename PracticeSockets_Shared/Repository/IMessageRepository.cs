using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeSockets_Shared.Models;

namespace PracticeSockets_Shared.Repository
{
    public interface IMessageRepository
    {
        public Task<Message> Get(int id);
        public IEnumerable<Message> GetAll();
        public Task Create(Message message);
        public Task Update(Message message);
    }
}
