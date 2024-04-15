using indie_hub_server.Dtos.User;
using indie_hub_server.Models;
using indie_hub_server.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace indie_hub_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Select(user => new UserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> PostUser([FromBody] CreateUserDTO newUser)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
            if (emailExists)
            {
                return BadRequest("Email already in use.");
            }

            UserService userService = new UserService();
            var user = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = userService.HashPassword(null, newUser.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UpdateUserDTO updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (_context.Users.Any(u => u.Email == updatedUser.Email && u.Id != id))
            {
                return BadRequest("Email already in use by another account.");
            }

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                UserService userService = new UserService();
                user.PasswordHash = userService.HashPassword(user, updatedUser.Password);
                _context.Entry(user).Property(u => u.PasswordHash).IsModified = true;
            }

            _context.Entry(user).Property(u => u.Username).IsModified = true;
            _context.Entry(user).Property(u => u.Email).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
