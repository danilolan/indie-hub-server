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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaytestResponseDTO>>> GetAllPlaytests([FromQuery] int page = 1, [FromQuery] int perPage = 10)
        {

            if (page < 1)
            {
                return BadRequest("Wrong 'page' param.");
            }

            if (perPage < 1)
            {
                return BadRequest("Wrong 'perPage' param.");
            }

            var playtests = await _context.Playtests
                .Include(p => p.User)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(p => new PlaytestResponseDTO
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    Title = p.Title,
                    Description = p.Description,
                    GameFileUrl = p.GameFileUrl,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Author = new PlaytestUserDTO
                    {
                        Id = p.User.Id,
                        Username = p.User.Username
                    }
                })
                .ToListAsync();

            return Ok(playtests);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaytest(int id, [FromBody] PlaytestUpdateDTO playtestDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("No valid token provided.");
            }

            var playtest = await _context.Playtests.FindAsync(id);

            if (playtest == null)
            {
                return NotFound();
            }

            if (playtest.UserId.ToString() != userId)
            {
                return Forbid("You are not authorized to update this playtest.");
            }

            playtest.Title = playtestDTO.Title;
            playtest.Description = playtestDTO.Description;
            playtest.GameFileUrl = playtestDTO.GameFileUrl;
            playtest.StartDate = playtestDTO.StartDate;
            playtest.EndDate = playtestDTO.EndDate;

            _context.Entry(playtest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaytestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        private bool PlaytestExists(int id)
        {
            return _context.Playtests.Any(e => e.Id == id);
        }

    }
}
