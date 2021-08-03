using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: user
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userRepository.GetAll());
        }

        // GET: user/Id
        [HttpGet]
        public async Task<IActionResult> GetById(int userId)
        {
            User result = await _userRepository.Get(userId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET: user
        [HttpGet]
        public IActionResult GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest();

            User result = _userRepository.GetByUsername(username);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        // POST: user
        public async Task<IActionResult> Create([FromBody] User user)
        {
            await _userRepository.Create(user);

            return Ok();
        }
    }
}
