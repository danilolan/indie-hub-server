using indie_hub_server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indie_hub_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Users : Controller
    {
        private readonly AppDbContext _context;

        public Users(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet("rota1")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }
    }
}
