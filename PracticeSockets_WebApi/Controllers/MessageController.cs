using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        // GET: message
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_messageRepository.GetAll());
        }

        // POST: message
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Message message)
        {
            await _messageRepository.Create(message);

            return Ok();
        }
    }
}
