using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PracticeSockets_Shared.Models;
using PracticeSockets_Shared.Repository;

namespace PracticeSockets_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        // GET: group
        [HttpGet]
        public IActionResult Get()
        {
            List<GroupSQLite> result = _groupRepository.GetAll().ToList();
            List<Group> groups = result.Select(
                g => new Group()
                {
                    Id = g.Id,
                    UserIds = g.UserIds.Split(',').Select(u => Int32.Parse(u)).ToList()
                }).ToList();

            return Ok(groups);
        }

        // POST: group
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Group group)
        {
            GroupSQLite groupSQLite = new GroupSQLite()
            {
                Id = group.Id,
                UserIds = string.Join(',', group.UserIds.OrderBy(u => u))
            };
            await _groupRepository.Create(groupSQLite);

            return Ok();
        }
    }
}
