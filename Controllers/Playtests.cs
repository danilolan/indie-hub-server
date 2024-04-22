using indie_hub_server.Dtos.Playtest;
using indie_hub_server.Dtos.User;
using indie_hub_server.Models;
using indie_hub_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace indie_hub_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlaytestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaytestsController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PlaytestResponseDTO>> CreatePlaytest([FromBody] PlaytestCreateDTO playtestDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("No valid token provided.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var playtest = new Playtest
            {
                CreatedAt = DateTime.UtcNow,
                Title = playtestDTO.Title,
                Description = playtestDTO.Description,
                GameFileUrl = playtestDTO.GameFileUrl,
                StartDate = playtestDTO.StartDate,
                EndDate = playtestDTO.EndDate,
                User = user
            };

            _context.Playtests.Add(playtest);
            await _context.SaveChangesAsync();

            var playtestResponseDTO = new PlaytestResponseDTO
            {
                Id = playtest.Id,
                CreatedAt = playtest.CreatedAt,
                Title = playtest.Title,
                Description = playtest.Description,
                GameFileUrl = playtest.GameFileUrl,
                StartDate = playtest.StartDate,
                EndDate = playtest.EndDate,
                Author = new PlaytestUserDTO
                {
                    Id = user.Id,
                    Username = user.Username
                }
            };

            return Ok(playtestResponseDTO);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PlaytestResponseDTO>> GetPlaytest(int id)
        {
            var playtest = await _context.Playtests
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playtest == null)
            {
                return NotFound();
            }

            var playtestDTO = new PlaytestResponseDTO
            {
                Id = playtest.Id,
                CreatedAt = playtest.CreatedAt,
                Title = playtest.Title,
                Description = playtest.Description,
                GameFileUrl = playtest.GameFileUrl,
                StartDate = playtest.StartDate,
                EndDate = playtest.EndDate,
                Author = new PlaytestUserDTO
                {
                    Id = playtest.User.Id,
                    Username = playtest.User.Username
                }
            };

            return Ok(playtestDTO);
        }


    }
}
